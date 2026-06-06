using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Infrastructure.Security;

namespace FitnessTrainingApp.Data;

public static class SeedData
{
    private static readonly DateTime CreatedAt = new(2026, 5, 21, 0, 0, 0, DateTimeKind.Utc);
    private static readonly byte[] AdminSalt = Convert.FromBase64String("YWRtaW4tc2VlZC1zYWx0IQ==");
    private static readonly byte[] TrainerSalt = Convert.FromBase64String("dHJhaW5lci1zZWVkLTEhIQ==");
    private static readonly byte[] UserSalt = Convert.FromBase64String("dXNlci1zZWVkLXNhbHQhIQ==");

    public static IReadOnlyList<User> Users =>
    [
        new()
        {
            Id = 1,
            FullName = "System Administrator",
            Email = "admin@fit.local",
            PasswordHash = PasswordHasher.HashPassword("Admin12345", AdminSalt),
            Role = UserRole.Administrator,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 2,
            FullName = "Olena Trainer",
            Email = "trainer@fit.local",
            PasswordHash = PasswordHasher.HashPassword("Trainer12345", TrainerSalt),
            Role = UserRole.Trainer,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 3,
            FullName = "Demo User",
            Email = "user@fit.local",
            PasswordHash = PasswordHasher.HashPassword("User12345", UserSalt),
            Role = UserRole.User,
            CreatedAt = CreatedAt
        }
    ];

    public static IReadOnlyList<Exercise> Exercises =>
    [
        Exercise(1, "Bodyweight Squat", "A foundational lower-body movement that builds strength in the quadriceps, glutes and hips while teaching proper knee and torso control.", DifficultyLevel.Beginner, WorkoutType.Home, "No equipment", "Legs", "Keep your chest lifted, brace your core and keep your knees tracking in the same direction as your toes."),
        Exercise(2, "Plank", "A static core exercise that develops abdominal endurance, shoulder stability and the ability to maintain a neutral spine under tension.", DifficultyLevel.Beginner, WorkoutType.Home, "Mat", "Core", "Avoid dropping the hips or lifting them too high; stop if you feel pressure in the lower back."),
        Exercise(3, "Dumbbell Row", "A unilateral pulling exercise that strengthens the upper back, lats and arms while improving control on each side of the body.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Dumbbells", "Back", "Keep the supporting shoulder stable and pull the dumbbell without rotating your torso."),
        Exercise(4, "Bench Press", "A classic compound press for developing chest, shoulder and triceps strength with a stable bar path and controlled tempo.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Barbell", "Chest", "Use a spotter for heavy sets and keep your feet, upper back and hips stable on the bench."),
        Exercise(5, "Deadlift", "A full-body strength exercise that trains the posterior chain, grip and trunk bracing through a hip-dominant lifting pattern.", DifficultyLevel.Advanced, WorkoutType.Gym, "Barbell", "Back", "Brace before each repetition, keep the bar close to the body and avoid rounding the lower back."),
        Exercise(6, "Push-up", "A bodyweight pressing exercise for the chest, shoulders, triceps and core that can be scaled for different fitness levels.", DifficultyLevel.Beginner, WorkoutType.Home, "No equipment", "Chest", "Keep the body in one straight line and lower under control instead of collapsing into the shoulders."),
        Exercise(7, "Glute Bridge", "A beginner-friendly hip extension movement that activates the glutes and teaches pelvic control without loading the spine heavily.", DifficultyLevel.Beginner, WorkoutType.Home, "Mat", "Glutes", "Do not overarch the lower back at the top; squeeze the glutes and keep the ribs down."),
        Exercise(8, "Reverse Lunge", "A single-leg lower-body exercise that improves balance, leg strength and hip control with less forward knee stress than walking lunges.", DifficultyLevel.Beginner, WorkoutType.Home, "No equipment", "Legs", "Step back far enough to keep the front heel grounded and descend with control."),
        Exercise(9, "Mountain Climber", "A dynamic bodyweight drill that combines core stability with light conditioning by driving the knees from a plank position.", DifficultyLevel.Beginner, WorkoutType.Home, "No equipment", "Core", "Keep the shoulders stacked over the wrists and avoid bouncing the hips excessively."),
        Exercise(10, "Side Plank", "A lateral core exercise that strengthens the obliques, hips and shoulder stabilizers while resisting side bending.", DifficultyLevel.Beginner, WorkoutType.Home, "Mat", "Core", "Keep the body aligned from head to feet and do not let the shoulder sink toward the ear."),
        Exercise(11, "Bird Dog", "A controlled core and back stability exercise that trains coordination between the hips, spine and shoulders.", DifficultyLevel.Beginner, WorkoutType.Home, "Mat", "Core", "Move slowly and keep the hips level instead of rotating when the arm and leg extend."),
        Exercise(12, "Standing Calf Raise", "A simple lower-leg exercise for strengthening the calves and improving ankle control during running, jumping and daily movement.", DifficultyLevel.Beginner, WorkoutType.Home, "No equipment", "Calves", "Rise through the full range of motion and lower slowly without rolling the ankles outward."),
        Exercise(13, "Step-up", "A practical single-leg exercise that builds leg strength, balance and hip stability using a bench, box or sturdy step.", DifficultyLevel.Beginner, WorkoutType.Home, "Bench", "Legs", "Place the full foot on the step and drive through the heel without pushing off strongly from the rear leg."),
        Exercise(14, "Hip Hinge Drill", "A technique exercise for learning how to bend at the hips while keeping the spine neutral before progressing to loaded pulls.", DifficultyLevel.Beginner, WorkoutType.Home, "No equipment", "Back", "Push the hips back first and keep a long spine throughout the movement."),
        Exercise(15, "Wall Sit", "An isometric lower-body exercise that builds endurance in the quadriceps and glutes with minimal equipment.", DifficultyLevel.Beginner, WorkoutType.Home, "Wall", "Legs", "Keep the knees aligned over the feet and stop if knee pain appears."),
        Exercise(16, "Jumping Jack", "A simple cardio movement that raises heart rate, warms up the shoulders and hips and prepares the body for training.", DifficultyLevel.Beginner, WorkoutType.Home, "No equipment", "Full body", "Land softly on the balls of the feet and keep the knees relaxed."),
        Exercise(17, "Superman Hold", "A posterior-chain endurance exercise that targets the lower back, glutes and upper back with a controlled floor hold.", DifficultyLevel.Beginner, WorkoutType.Home, "Mat", "Back", "Lift only as high as you can without pinching the lower back or holding your breath."),
        Exercise(18, "Hollow Body Hold", "A core tension drill that teaches full-body bracing and improves control for more advanced bodyweight movements.", DifficultyLevel.Intermediate, WorkoutType.Home, "Mat", "Core", "Press the lower back into the floor and shorten the hold if you lose position."),
        Exercise(19, "Incline Push-up", "A scaled pressing variation that reduces bodyweight load while building proper push-up mechanics and upper-body strength.", DifficultyLevel.Beginner, WorkoutType.Home, "Bench", "Chest", "Use a stable surface and keep the elbows controlled rather than flaring aggressively."),
        Exercise(20, "Single-leg Romanian Deadlift", "A balance-focused hinge exercise that strengthens the hamstrings, glutes and stabilizing muscles around the hips.", DifficultyLevel.Intermediate, WorkoutType.Home, "Dumbbells", "Hamstrings", "Keep the hips square to the floor and use a light load until balance is consistent."),
        Exercise(21, "Goblet Squat", "A loaded squat variation that helps reinforce upright posture while strengthening the legs and core with a front-held weight.", DifficultyLevel.Beginner, WorkoutType.Gym, "Dumbbell", "Legs", "Hold the weight close to the chest and keep the elbows inside the knees at the bottom."),
        Exercise(22, "Dumbbell Shoulder Press", "An overhead pressing exercise for the shoulders and triceps that also requires core control in a seated or standing position.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Dumbbells", "Shoulders", "Avoid leaning back as the weights move overhead and keep the wrists stacked over the elbows."),
        Exercise(23, "Lat Pulldown", "A machine-based vertical pulling exercise that develops the lats, upper back and pulling strength for future pull-up progressions.", DifficultyLevel.Beginner, WorkoutType.Gym, "Cable machine", "Back", "Pull the bar toward the upper chest and avoid yanking it behind the neck."),
        Exercise(24, "Seated Cable Row", "A horizontal pulling movement that strengthens the mid-back, lats and posture muscles with steady cable resistance.", DifficultyLevel.Beginner, WorkoutType.Gym, "Cable machine", "Back", "Keep the torso tall and pull with the elbows instead of leaning far backward."),
        Exercise(25, "Leg Press", "A machine lower-body exercise for training the quadriceps, glutes and hamstrings with a guided movement path.", DifficultyLevel.Beginner, WorkoutType.Gym, "Leg press machine", "Legs", "Do not lock the knees forcefully at the top and keep the lower back supported."),
        Exercise(26, "Romanian Deadlift", "A hip-hinge strength exercise that emphasizes the hamstrings and glutes while maintaining tension through a partial range.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Barbell", "Hamstrings", "Keep the bar close to the legs and stop the descent when the back position starts to change."),
        Exercise(27, "Barbell Back Squat", "A major compound lift for building lower-body strength, trunk stability and coordinated movement under load.", DifficultyLevel.Advanced, WorkoutType.Gym, "Barbell", "Legs", "Use safety pins or a spotter, brace before descending and maintain control through the full repetition."),
        Exercise(28, "Walking Lunge", "A dynamic single-leg exercise that trains the quadriceps, glutes and balance while moving through space.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Dumbbells", "Legs", "Take consistent steps and avoid letting the front knee collapse inward."),
        Exercise(29, "Chest Fly", "An isolation exercise that targets the chest through a wide arc and complements heavier pressing movements.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Dumbbells", "Chest", "Use a moderate range of motion and keep a slight bend in the elbows."),
        Exercise(30, "Triceps Rope Pushdown", "A cable isolation exercise that strengthens the triceps and improves elbow extension control.", DifficultyLevel.Beginner, WorkoutType.Gym, "Cable machine", "Arms", "Keep the elbows close to the body and avoid using body momentum."),
        Exercise(31, "Biceps Curl", "A basic arm exercise for strengthening the biceps with controlled elbow flexion and stable shoulder position.", DifficultyLevel.Beginner, WorkoutType.Gym, "Dumbbells", "Arms", "Do not swing the torso; lift and lower the weight with control."),
        Exercise(32, "Face Pull", "A cable exercise for the rear shoulders and upper back that supports posture and shoulder health.", DifficultyLevel.Beginner, WorkoutType.Gym, "Cable machine", "Shoulders", "Pull toward eye level and keep the movement smooth rather than forcing heavy weight."),
        Exercise(33, "Kettlebell Swing", "A powerful hip-hinge movement that develops glutes, hamstrings and conditioning through explosive but controlled reps.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Kettlebell", "Full body", "Drive from the hips, not the lower back, and stop if the swing turns into a squat."),
        Exercise(34, "Pull-up", "A demanding bodyweight pulling exercise that builds back, arm and grip strength using a vertical pulling pattern.", DifficultyLevel.Advanced, WorkoutType.Gym, "Pull-up bar", "Back", "Start each repetition from control and avoid swinging unless the workout specifically requires it."),
        Exercise(35, "Assisted Pull-up", "A scaled pull-up variation that builds vertical pulling strength while reducing the amount of bodyweight lifted.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Assisted pull-up machine", "Back", "Choose assistance that lets you use full range without losing shoulder control."),
        Exercise(36, "Leg Curl", "A machine exercise that isolates the hamstrings and supports balanced lower-body strength development.", DifficultyLevel.Beginner, WorkoutType.Gym, "Leg curl machine", "Hamstrings", "Control the lowering phase and keep the hips pressed into the pad."),
        Exercise(37, "Leg Extension", "A machine exercise that targets the quadriceps through knee extension and is useful for controlled accessory work.", DifficultyLevel.Beginner, WorkoutType.Gym, "Leg extension machine", "Legs", "Use a comfortable range and avoid snapping the knees into lockout."),
        Exercise(38, "Cable Woodchop", "A rotational core exercise that trains the obliques and trunk control through a diagonal cable path.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Cable machine", "Core", "Rotate through the torso and hips while keeping the movement controlled."),
        Exercise(39, "Russian Twist", "A seated core exercise that emphasizes trunk rotation and abdominal endurance with or without added resistance.", DifficultyLevel.Intermediate, WorkoutType.Home, "Mat", "Core", "Keep the spine long and rotate under control instead of jerking side to side."),
        Exercise(40, "Hanging Knee Raise", "A core exercise performed from a hanging position to strengthen the abdominals and hip flexors.", DifficultyLevel.Advanced, WorkoutType.Gym, "Pull-up bar", "Core", "Avoid swinging and raise the knees with control before lowering slowly."),
        Exercise(41, "Box Jump", "A lower-body power exercise that trains explosive hip and knee extension with a controlled landing.", DifficultyLevel.Advanced, WorkoutType.Gym, "Box", "Legs", "Choose a box height you can land on softly and step down instead of jumping backward."),
        Exercise(42, "Battle Rope Waves", "A conditioning drill for the shoulders, arms and core that uses repeated rope waves to raise work capacity.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Battle ropes", "Full body", "Keep the knees soft and maintain posture instead of rounding forward."),
        Exercise(43, "Farmer's Carry", "A loaded carry that develops grip strength, core stability and posture while walking with heavy weights.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Dumbbells", "Full body", "Walk tall with the shoulders down and avoid leaning to one side."),
        Exercise(44, "Treadmill Incline Walk", "A low-impact cardio option that builds aerobic endurance and posterior-chain engagement through inclined walking.", DifficultyLevel.Beginner, WorkoutType.Gym, "Treadmill", "Cardio", "Use a pace that allows steady breathing and avoid holding the rails tightly."),
        Exercise(45, "Stationary Bike Intervals", "A joint-friendly conditioning exercise using alternating hard and easy efforts on a stationary bike.", DifficultyLevel.Beginner, WorkoutType.Gym, "Stationary bike", "Cardio", "Adjust the seat height and keep the knees tracking smoothly during each pedal stroke."),
        Exercise(46, "Burpee", "A full-body conditioning exercise that combines a squat, plank transition and jump for high-energy circuit training.", DifficultyLevel.Advanced, WorkoutType.Home, "No equipment", "Full body", "Move at a pace you can control and step back instead of jumping if form breaks down."),
        Exercise(47, "Medicine Ball Slam", "A power and conditioning movement that trains the core, shoulders and hips through an explosive downward throw.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Medicine ball", "Full body", "Use a slam-safe ball and keep the back neutral as you pick it up."),
        Exercise(48, "Single-arm Dumbbell Snatch", "An explosive full-body lift that moves a dumbbell from the floor to overhead in one coordinated motion.", DifficultyLevel.Advanced, WorkoutType.Gym, "Dumbbell", "Full body", "Start light, keep the dumbbell close and finish with a stable overhead position."),
        Exercise(49, "Bulgarian Split Squat", "A challenging single-leg exercise that targets the quadriceps and glutes while demanding balance and hip control.", DifficultyLevel.Advanced, WorkoutType.Gym, "Bench", "Legs", "Keep the front foot planted and lower only as far as you can control."),
        Exercise(50, "Barbell Hip Thrust", "A loaded glute-focused exercise that trains strong hip extension with the upper back supported on a bench.", DifficultyLevel.Intermediate, WorkoutType.Gym, "Barbell", "Glutes", "Keep the chin tucked, ribs down and pause briefly at the top without overextending the back.")
    ];

    public static IReadOnlyList<MediaFile> MediaFiles =>
    [
        Media(1, 1, "bodyweight-squat", "fitness,squat"),
        Media(2, 2, "plank", "fitness,plank"),
        Media(3, 3, "dumbbell-row", "fitness,dumbbell,row"),
        Media(4, 4, "bench-press", "fitness,bench,press"),
        Media(5, 5, "deadlift", "fitness,deadlift"),
        Media(6, 6, "push-up", "fitness,pushup"),
        Media(7, 7, "glute-bridge", "fitness,glute,bridge"),
        Media(8, 8, "reverse-lunge", "fitness,lunge"),
        Media(9, 9, "mountain-climber", "fitness,core,training"),
        Media(10, 10, "side-plank", "fitness,side,plank"),
        Media(11, 11, "bird-dog", "fitness,stretching"),
        Media(12, 12, "standing-calf-raise", "fitness,legs"),
        Media(13, 13, "step-up", "fitness,step,exercise"),
        Media(14, 14, "hip-hinge-drill", "fitness,hinge"),
        Media(15, 15, "wall-sit", "fitness,wall,sit"),
        Media(16, 16, "jumping-jack", "fitness,cardio"),
        Media(17, 17, "superman-hold", "fitness,back,exercise"),
        Media(18, 18, "hollow-body-hold", "fitness,core"),
        Media(19, 19, "incline-push-up", "fitness,pushup"),
        Media(20, 20, "single-leg-romanian-deadlift", "fitness,balance,dumbbell"),
        Media(21, 21, "goblet-squat", "fitness,dumbbell,squat"),
        Media(22, 22, "dumbbell-shoulder-press", "fitness,shoulder,press"),
        Media(23, 23, "lat-pulldown", "fitness,gym,pulldown"),
        Media(24, 24, "seated-cable-row", "fitness,cable,row"),
        Media(25, 25, "leg-press", "fitness,leg,press"),
        Media(26, 26, "romanian-deadlift", "fitness,barbell,training"),
        Media(27, 27, "barbell-back-squat", "fitness,barbell,squat"),
        Media(28, 28, "walking-lunge", "fitness,lunge,dumbbell"),
        Media(29, 29, "chest-fly", "fitness,chest,dumbbell"),
        Media(30, 30, "triceps-rope-pushdown", "fitness,cable,triceps"),
        Media(31, 31, "biceps-curl", "fitness,biceps,curl"),
        Media(32, 32, "face-pull", "fitness,shoulder,cable"),
        Media(33, 33, "kettlebell-swing", "fitness,kettlebell"),
        Media(34, 34, "pull-up", "fitness,pullup"),
        Media(35, 35, "assisted-pull-up", "fitness,pullup,gym"),
        Media(36, 36, "leg-curl", "fitness,hamstrings"),
        Media(37, 37, "leg-extension", "fitness,leg,machine"),
        Media(38, 38, "cable-woodchop", "fitness,cable,core"),
        Media(39, 39, "russian-twist", "fitness,core,mat"),
        Media(40, 40, "hanging-knee-raise", "fitness,core,pullup"),
        Media(41, 41, "box-jump", "fitness,box,jump"),
        Media(42, 42, "battle-rope-waves", "fitness,battle,ropes"),
        Media(43, 43, "farmers-carry", "fitness,dumbbells,carry"),
        Media(44, 44, "treadmill-incline-walk", "fitness,treadmill"),
        Media(45, 45, "stationary-bike-intervals", "fitness,cycling"),
        Media(46, 46, "burpee", "fitness,burpee"),
        Media(47, 47, "medicine-ball-slam", "fitness,medicine,ball"),
        Media(48, 48, "single-arm-dumbbell-snatch", "fitness,dumbbell,snatch"),
        Media(49, 49, "bulgarian-split-squat", "fitness,split,squat"),
        Media(50, 50, "barbell-hip-thrust", "fitness,hip,thrust")
    ];

    public static IReadOnlyList<WorkoutComplex> WorkoutComplexes =>
    [
        Complex(1, "Home Starter Plan", "A simple beginner-friendly home workout for building consistency without gym equipment.", DifficultyLevel.Beginner, WorkoutType.Home, 25),
        Complex(2, "Gym Strength Base", "A structured gym workout focused on foundational compound strength exercises.", DifficultyLevel.Intermediate, WorkoutType.Gym, 50),
        Complex(3, "Upper Body Push", "A focused chest and triceps workout built around pressing strength and shoulder control.", DifficultyLevel.Intermediate, WorkoutType.Gym, 40),
        Complex(4, "Home Full Body Beginner", "A balanced low-barrier session for users who want to train the whole body at home with simple bodyweight movements.", DifficultyLevel.Beginner, WorkoutType.Home, 30),
        Complex(5, "Core Stability Basics", "A short core-focused plan for improving trunk endurance, posture and control before progressing to harder movements.", DifficultyLevel.Beginner, WorkoutType.Home, 22),
        Complex(6, "Lower Body Home Strength", "A home lower-body workout focused on squats, lunges, glute work and controlled single-leg strength.", DifficultyLevel.Beginner, WorkoutType.Home, 35),
        Complex(7, "Gym Upper Body Pull", "A gym session for back and biceps development using vertical pulls, rows and posture-focused accessory work.", DifficultyLevel.Intermediate, WorkoutType.Gym, 45),
        Complex(8, "Gym Lower Body Strength", "A lower-body gym plan built around squats, hinges and machine accessories for complete leg development.", DifficultyLevel.Intermediate, WorkoutType.Gym, 55),
        Complex(9, "Push Pull Gym Session", "A combined upper-body workout that alternates pressing and pulling patterns for balanced strength training.", DifficultyLevel.Intermediate, WorkoutType.Gym, 50),
        Complex(10, "Advanced Barbell Strength", "A demanding strength plan for experienced users who can safely handle heavier barbell movements.", DifficultyLevel.Advanced, WorkoutType.Gym, 65),
        Complex(11, "Home Conditioning Circuit", "A fast-paced home workout that combines bodyweight strength and conditioning for users with limited time.", DifficultyLevel.Intermediate, WorkoutType.Home, 28),
        Complex(12, "Mobility and Core Reset", "A lighter session for active recovery, core control and movement quality between harder training days.", DifficultyLevel.Beginner, WorkoutType.Home, 25),
        Complex(13, "Glutes and Legs Builder", "A gym plan targeting glutes, hamstrings and quadriceps with a mix of compound and accessory exercises.", DifficultyLevel.Intermediate, WorkoutType.Gym, 52),
        Complex(14, "Cardio Fat Burn Mix", "A conditioning-focused plan that combines low-impact cardio with short bodyweight intervals.", DifficultyLevel.Beginner, WorkoutType.Gym, 35),
        Complex(15, "Athletic Power Session", "An advanced session for explosive power, loaded carries and high-output conditioning in the gym.", DifficultyLevel.Advanced, WorkoutType.Gym, 48)
    ];

    public static IReadOnlyList<WorkoutComplexExercise> WorkoutComplexExercises =>
    [
        Link(1, 1, 1, 3, 12), Link(1, 6, 2, 3, 10), Link(1, 2, 3, 3, 45), Link(1, 7, 4, 3, 15),
        Link(2, 27, 1, 4, 6), Link(2, 4, 2, 4, 8), Link(2, 5, 3, 4, 5), Link(2, 3, 4, 3, 10), Link(2, 43, 5, 3, 40),
        Link(3, 4, 1, 4, 8), Link(3, 22, 2, 3, 10), Link(3, 29, 3, 3, 12), Link(3, 30, 4, 3, 12), Link(3, 6, 5, 3, 12),
        Link(4, 16, 1, 3, 30), Link(4, 1, 2, 3, 15), Link(4, 19, 3, 3, 12), Link(4, 8, 4, 3, 10), Link(4, 2, 5, 3, 40),
        Link(5, 11, 1, 3, 12), Link(5, 2, 2, 3, 45), Link(5, 10, 3, 3, 30), Link(5, 18, 4, 3, 25), Link(5, 39, 5, 3, 20),
        Link(6, 1, 1, 4, 12), Link(6, 8, 2, 3, 10), Link(6, 13, 3, 3, 12), Link(6, 7, 4, 4, 15), Link(6, 15, 5, 3, 45),
        Link(7, 23, 1, 4, 10), Link(7, 24, 2, 4, 10), Link(7, 35, 3, 3, 8), Link(7, 32, 4, 3, 15), Link(7, 31, 5, 3, 12),
        Link(8, 27, 1, 4, 6), Link(8, 26, 2, 4, 8), Link(8, 25, 3, 3, 12), Link(8, 36, 4, 3, 12), Link(8, 37, 5, 3, 12),
        Link(9, 4, 1, 4, 8), Link(9, 24, 2, 4, 10), Link(9, 22, 3, 3, 10), Link(9, 23, 4, 3, 10), Link(9, 32, 5, 3, 15),
        Link(10, 27, 1, 5, 5), Link(10, 5, 2, 5, 4), Link(10, 4, 3, 5, 5), Link(10, 34, 4, 4, 6), Link(10, 48, 5, 3, 6),
        Link(11, 16, 1, 4, 40), Link(11, 46, 2, 4, 8), Link(11, 9, 3, 4, 30), Link(11, 6, 4, 4, 12), Link(11, 39, 5, 4, 20),
        Link(12, 14, 1, 3, 12), Link(12, 11, 2, 3, 12), Link(12, 17, 3, 3, 20), Link(12, 10, 4, 3, 25), Link(12, 12, 5, 3, 15),
        Link(13, 50, 1, 4, 10), Link(13, 49, 2, 3, 10), Link(13, 28, 3, 3, 12), Link(13, 36, 4, 3, 12), Link(13, 33, 5, 3, 15),
        Link(14, 44, 1, 1, 15), Link(14, 45, 2, 8, 30), Link(14, 42, 3, 5, 30), Link(14, 47, 4, 4, 12), Link(14, 16, 5, 4, 35),
        Link(15, 41, 1, 4, 5), Link(15, 48, 2, 4, 6), Link(15, 33, 3, 4, 12), Link(15, 43, 4, 4, 40), Link(15, 42, 5, 5, 30)
    ];

    private static Exercise Exercise(
        int id,
        string name,
        string description,
        DifficultyLevel difficulty,
        WorkoutType workoutType,
        string equipment,
        string muscleGroup,
        string safetyNotes) =>
        new()
        {
            Id = id,
            Name = name,
            Description = description,
            Difficulty = difficulty,
            WorkoutType = workoutType,
            Equipment = equipment,
            MuscleGroup = muscleGroup,
            SafetyNotes = safetyNotes,
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        };

    private static MediaFile Media(int id, int exerciseId, string fileName, string query) =>
        new()
        {
            Id = id,
            ExerciseId = exerciseId,
            FileName = $"{fileName}.jpg",
            Url = $"https://loremflickr.com/1200/800/{query}/all?lock={id}",
            ContentType = "image/jpeg",
            CreatedAt = CreatedAt
        };

    private static WorkoutComplex Complex(
        int id,
        string name,
        string description,
        DifficultyLevel difficulty,
        WorkoutType workoutType,
        int durationMinutes) =>
        new()
        {
            Id = id,
            Name = name,
            Description = description,
            Difficulty = difficulty,
            WorkoutType = workoutType,
            DurationMinutes = durationMinutes,
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        };

    private static WorkoutComplexExercise Link(int complexId, int exerciseId, int order, int sets, int repetitions) =>
        new()
        {
            WorkoutComplexId = complexId,
            ExerciseId = exerciseId,
            OrderNumber = order,
            Sets = sets,
            Repetitions = repetitions
        };
}
