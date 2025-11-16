# Basic GOAP for RTS

RTS won't stay true to original Goal Oriented Action Plan, because it will be adapted to the specific needs.

## Sample End Goal

This will be a basic example of an RTS, except that it runs entirely on its own. Like an enemy AI. This will serve as a test bed for AI development, and will also serve as a good example. 

1. Begin with Workshop and Resources of 100 credits
2. What ever goal is in place, it should evaluate it needs a worker and construct one, then send it to collect resources. 
3. Work Shop Constructs Worker
4. Worker goes to mine, dissapears, reappears with material, returns to work shop. Loops, credit count rises. 
5. After enough, it will decide to build another worker to have it collect. This repeats a few times to build up resources. 
6. At a certain point of growth, it determines to build a barracks - automatically selecting a worker to do it. 
7. After barracks, barracks builds up a small force. to defend, and sends a scout or two to evaluate the area.
8. On interaction with other base, extra offense/defense tries to take out opponent. 

To achieve this, we will need several systems in place, though minimally only. 

1. **Multiple Players**
- Multiple players will be operating in the same space, and so systems for managing their AI and resource information need to be separated. 
- Because this is used for AI preparation, we need to be able to compare AI's, so we need to be able to select different intellegences in GOAP selection, planning, Unit availability, ets.

2. **Units**
- Units include all controllable entities including characters and buildings. They differ only in abilities. 
- An Ability may affect menu choices, but certainly AI features. 

3. **View and Data**
- Data and controllers will manage the data in a separate mentality, so that the game actually plays out in the higher performance low level memory structures, and the view just displays the active state. 
- We will use mesh instance drawing to allow the system to generate this on its own. We will just need to handle our own occlusion, at minimum based on camera frustrum. 