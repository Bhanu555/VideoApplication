<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VideoApplication._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css" media="screen">
        html, body {
            height: 95%;
        }

        body {
            margin: 0;
            padding: 0;
            overflow: auto;
            text-align: left;
            background-color: Gray;
        }

        object:focus {
            outline: none;
        }

        #flashContent {
            display: none;
        }


        #video {
            position: relative;
            top: 17px;
            left: 71px;
            color: #99FF33;
        }

        #text {
            position: relative;
            z-index: 2147483647;
            top: 31px;
            left: 0px;
        }

        #remote-media {
            position: absolute;
            top: 150px;
            bottom: 20%;
            width:50%;
            height: 100%;
            overflow: hidden;
        }

            #remote-media video {
                /* Make video to at least 100% wide and tall */
                min-width: 100%;
                min-height: 100%;
                /* Setting width & height to auto prevents the browser from stretching or squishing the video */
                width: auto;
                height: auto;
                /* Center the video */
                position: absolute;
                top: 50%;
                left: 50%;
                transform: translate(-50%,-50%);
            }
        /*#local-media {
                width: 100%
            }*/
        #container body-content{
            margin: 0px;
        }
        #local-media {
            height: 169px;
            left: 100px;
            top: 900px;
            width: 280px;
            border: 5px hidden solid white;
            position: absolute;
        }
        #local-media{
            position: absolute;
        }

        .auto-style1 {
            color: #00FF00;
            width: 233px;
        }

        .overlay {
            position: absolute;
            top: 11%;
            left: 19%;
            width: 200%;
            height: 151%;
            z-index: 10;
            background-color: rgba(0,0,0,0.5); /*dim the background*/
        }

        .auto-style2 {
            color: #99FF33;
            width: 703px;
        }
        .myButton {background-color: Green; color: White;}
    </style>

    <p class="auto-style1"><strong>Hola!!! 
        </strong></p>
    <h1 class="auto-style2">Welcome To Twilio Programmable Video</h1>
    <asp:Button ID="RecordButton" Text="Click here for recording of specified Room" OnClick="RecordingBtnn_Click" class="myButton" runat="server" />
    <div id="picture-in-picture">
        <div id="remote-media"><strong> 
    </strong></div>
        <div id="controls">
            <div id="preview">
                <div id="local-media"></div>
                <br />
            </div>
            <div id="room-controls">
                <button type="button" id="button-leave" style="display: none;"></button>
            </div>
            <div id="log"></div>

        </div>
    </div>
    <div id="text">
    </div>
    <script src="//media.twiliocdn.com/sdk/js/video/v1/twilio-video.min.js"></script>
    <script type="text/javascript">      
        var activeRoom;
        var previewTracks;

        var identity = 'Bhanu';
        var roomName;
        var connectOptions = {
            name: 'Alice',
            logLevel: 'debug',
            audio: true,
            video: {
                width: 400,
                height: 300,

            },
        };

        if (previewTracks) {
            connectOptions.tracks = previewTracks;
        }

        // Join the Room with the token from the server and the
        // LocalParticipant's Tracks.
        Twilio.Video.connect('<%:VideoApplication.Utility1.TokenGenerator.AccessTokenGenerator() %>', connectOptions)
            .then(roomJoined, function (error) {
                console.error('Unable to connect to Room: ' + error.message);
            });

        // Bind button to leave Room.
        document.getElementById('button-leave').onclick = function (event) {
            event.preventDefault()
            console.log('Leaving room...');
            activeRoom.disconnect();
<%--             __doPostBack("<%= RecordButton.UniqueID %>", "OnClick");--%>
<%--         document.getElementById('<%= RecordButton.ClientID %>').click();--%>

        };

        function attachTracks(tracks, container) {
            tracks.forEach(function (track) {
                container.appendChild(track.attach());
            });
        }

        // Attach the Participant's Tracks to the DOM.
        function attachParticipantTracks(participant, container) {
            var tracks = Array.from(participant.tracks.values());
            attachTracks(tracks, container);
        };

        // Detach the Tracks from the DOM.
        function detachTracks(tracks) {
            tracks.forEach(function (track) {
                track.detach().forEach(function (detachedElement) {
                    detachedElement.remove();
                });
            });
        };

        // Detach the Participant's Tracks from the DOM.
        function detachParticipantTracks(participant) {
            var tracks = Array.from(participant.tracks.values());
            detachTracks(tracks);
        };

        function roomJoined(room) {
            window.room = activeRoom = room;

            /* 
 
            log("Joined as '" + identity + "'"); */
            document.getElementById('button-leave').style.display = 'none';

            // Attach LocalParticipant's Tracks, if not already attached.
            var previewContainer = document.getElementById('local-media');
            if (!previewContainer.querySelector('video')) {
                attachParticipantTracks(room.localParticipant, previewContainer);
            }

            // Attach the Tracks of the Room's Participants.
            room.participants.forEach(function (participant) {
                console.log("Already in Room: '" + participant.identity + "'");
                var previewContainer = document.getElementById('remote-media');
                attachParticipantTracks(participant, previewContainer);
            });

            // When a Participant joins the Room, log the event.
            room.on('participantConnected', function (participant) {
                console.log("Joining: '" + participant.identity + "'");
            });

            // When a Participant adds a Track, attach it to the DOM.
            room.on('trackAdded', function (track, participant) {
                console.log(participant.identity + " added track: " + track.kind);
                var previewContainer = document.getElementById('remote-media');
                attachTracks([track], previewContainer);
            });

            // When a Participant removes a Track, detach it from the DOM.
            room.on('trackRemoved', function (track, participant) {
                console.log(participant.identity + " removed track: " + track.kind);
                detachTracks([track]);
            });

            // When a Participant leaves the Room, detach its Tracks.
            room.on('participantDisconnected', function (participant) {
                console.log("Participant '" + participant.identity + "' left the room");
                detachParticipantTracks(participant);
            });

            // Once the LocalParticipant leaves the room, detach the Tracks
            // of all Participants, including that of the LocalParticipant.
            room.on('disconnected', function () {
                console.log('Left');
                if (previewTracks) {
                    previewTracks.forEach(function (track) {
                        track.stop();
                    });
                }
                detachParticipantTracks(room.localParticipant);
                room.participants.forEach(detachParticipantTracks);
                activeRoom = null;
                document.getElementById('button-leave').style.display = 'none';
            });
        }
    </script>
    
</asp:Content>
