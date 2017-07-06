# PeopleCounter
Count people using EmguCV(OpenCV) and cameras by face detection and recognition

# Idea
Count poeple via face recognition using multiple cameras to detect people when they enter or leave a specific zone

![alt text](https://s24.postimg.org/4vl7m5clh/Untitled.png "")


Use two or more cameras to detect people entering and leaving from a building.

Entered poeple are saved temporarily until they leave so we have a way to track them so same persons are not counted multiple times.

# Enter camera
1. New persons face detected.
2. Check if person is already inside(counted on previous frame) if not add count and save person

# Leave Camera
1. Person face detected.
2. Check if person is inside, if so remove count, remove person from inside list. If person not found inside then do nothing

