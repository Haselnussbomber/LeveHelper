using System.Collections.Generic;

namespace LeveHelper;

public static class Data
{
    public static readonly Dictionary<uint, uint[]> Issuers = new()
    {
        // Gontrant
        [1000101] = [
            21, // In with the New
            22, // Touch and Heal
            23, // A Clogful of Camaraderie
            27, // Bows for the Boys
            28, // Shields for the Serpents
            29, // Spears and Sorcery
            33, // Citizens' Canes
            34, // On the Move
            35, // Raise the Roof
            39, // Re-crating the Scene
            40, // Nightmare on My Street
            41, // Driving Up the Wall
            45, // As the Worm Turns
            46, // Behind the Mask
            47, // Militia on My Mind
            51, // Daddy's Little Girl
            52, // Tools for the Tools
            53, // Armoires of the Rich and Famous
            57, // Knock on Wood
            58, // An Expected Tourney
            59, // Ceremonial Spears
            63, // Stay on Target
            64, // A Tree Grew in Gridania
            65, // Flintstone Fight
            69, // The Arsenal of Theocracy
            70, // Greenstone for Greenhorns
            71, // Spin It Like You Mean It
            75, // Trident and Error
            76, // Clogs of War
            77, // You Do the Heavy Lifting
            81, // Red in the Head
            82, // Underneath It All
            83, // Sole Traders
            87, // Mind over Muzzle
            88, // From the Sands to the Stage
            89, // In Their Shoes
            93, // Quelling Bloody Rumors
            94, // The Bards' Guards
            95, // Saddle Sore
            99, // Choke Hold
            100, // Heads Up
            101, // Skin off Their Backs
            105, // Playing the Part
            106, // Fire and Hide
            107, // My Sole to Take
            111, // Men Who Scare Up Goats
            112, // Just Rewards
            113, // Breeches Served Cold
            117, // Emergency Patches
            118, // Quality over Quantity
            119, // Best Served Toad
            123, // The Righteous Tools for the Job
            124, // Soft Shoe Shuffle
            125, // Supply Side Logic
            129, // The Birdmen of Ishgard
            130, // Skirt Chaser
            131, // The Tao of Rabbits
            135, // Hold On Tight
            136, // Too Hot to Handle
            137, // Handle with Care
            501, // Water, Water Everywhere
            503, // Scourge of the Saplings
            504, // Stay for a Vile
            505, // Nutbreaker Suite
            506, // Picking Up the Piercers
            520, // A Vine Finer than Twine
            521, // River Raid
            522, // A Full Stomach
            523, // Wrangling Raptors
            524, // Compost with the Most
            537, // Going Green
            538, // Infestation Cessation
            539, // Turning Out the Blight
            540, // Bump in the Night
            541, // Wail of a Tale
            545, // A Shroom with a View
            567, // Monkey Business
            568, // Something in the Mead
            569, // Can't Say No to Gnat
            570, // Ochus Bite, Leaves Bleed
            571, // Stew for Two
            572, // A Guest for Supper
            589, // Belly Up
            590, // No Leaves Left Behind
            591, // What's Yours Is Mine
            592, // The Root of the Problem
            593, // Killing Beasts Softly
            594, // It's a Trap
            604, // Refugee Raw
            605, // More than Meets the Eye
            606, // Necrologos: Olidious Separation
            607, // Revisiting Raimdelle
            608, // Up the Creek
            609, // Rope a 'Lope
            617, // A Feast in the Forest
            618, // Black Market Down
            619, // Necrologos: Brand of the Impure
            620, // Adamantoise Tears
            621, // Woodcross Busydeal
            674, // Waking Wood
            675, // East Bank Story
            676, // We Couldn't Start the Fire
            677, // Lovely Latex
            678, // Let the Sun Shine In
            679, // If a Tree Falls
            680, // Evil Seeds
            681, // No Bark, All Bite
            682, // A Chest of Nuts
            683, // West Bank Story
            684, // Digging Deep
            685, // The Heart of the Hedge
            686, // The Quick and the Dead
            687, // Maple Stories
            688, // Never Strikes Twice
            689, // Briar in the Hole
            690, // Nowhere to Slide
            691, // Shaken, Not Stirred
            692, // Just the Artifacts, Madam
            693, // Fueling the Flames
            694, // Over the Underbrush
            695, // Moon in Rouge
            696, // Appleanche
            697, // Mushroom Gobblin'
            730, // Look How They Shine for You
            731, // Elemental Housekeeping
            733, // Rock My Wall
            734, // Fool Me Twice
            735, // Can't Start a Fire
            737, // Baby, Light My Way
            770, // Sounds Fishy to Me
            771, // The Long and the Shortcrust
            772, // Food Chain Reaction
            773, // Blind Ambition
            774, // A Shocking Soirée
            775, // A Watery Web of Lies
            776, // Fishing 101
            777, // The Truth Will Set You Free
        ],
        // Tierney
        [1000105] = [
            36, // Say It with Spears
            37, // Life's a Stitch
            38, // Shielding Sales
            96, // Only the Best
            97, // Simply the Best
            98, // Targe Up
            537, // Going Green
            538, // Infestation Cessation
            539, // Turning Out the Blight
            540, // Bump in the Night
            541, // Wail of a Tale
            543, // Scent of a Roselet
            545, // A Shroom with a View
            682, // A Chest of Nuts
            683, // West Bank Story
            684, // Digging Deep
            685, // The Heart of the Hedge
        ],
        // Qina Lyehga
        [1000821] = [
            42, // Nothing to Hide
            43, // What You Need
            44, // Bowing to Necessity
            102, // Back in the Band
            103, // On Their Feet Again
            104, // A Rush on Ringbands
            567, // Monkey Business
            568, // Something in the Mead
            569, // Can't Say No to Gnat
            570, // Ochus Bite, Leaves Bleed
            571, // Stew for Two
            572, // A Guest for Supper
            686, // The Quick and the Dead
            687, // Maple Stories
            689, // Briar in the Hole
        ],
        // Nyell
        [1000823] = [
            48, // Grinding It Out
            49, // Polearms Aplenty
            50, // Wall Not Found
            54, // Storm of Swords
            55, // Toys of Summer
            56, // Heal Away
            108, // Hands On
            109, // Packing a Punch
            110, // Open to Attack
            114, // No Risk, No Reward
            115, // Campaign in the Membrane
            116, // Quicker than Sand
            168, // Powderpost Derby
            169, // The Devil's Workshop
            170, // When Rhalgr Met Nophica
            174, // Lancers' Creed
            175, // I Saw What You Did There
            176, // That's Some Fine Grinding
            228, // No Hand-me-downs
            229, // Not Enough Headroom
            230, // I Was a Teenage Wailer
            234, // Need for Mead
            235, // Hot for Teacher
            236, // Get Shirty
            288, // For Crumbs' Sake
            289, // Picnic Panic
            290, // Food Fight
            294, // Whirled Peas
            295, // Love's Crumpets Lost
            296, // I Love Lamprey
            348, // Dripping with Venom
            349, // Blind Man's Bluff
            350, // Hush Little Wailer
            354, // Conspicuous Conjuration
            355, // You Put Your Left Hand In
            356, // The Wailers' First Law of Potion
            408, // Getting Handsy
            409, // The Telltale Tress
            410, // Whatchoo Talking About
            414, // Pantser Corps
            415, // Put a Lid on It
            416, // Bet You Anything
            468, // Music to Their Ears
            469, // Dog Tags Are for Dogs
            470, // One and Only
            474, // Necklet of Champions
            475, // Keep the Change
            476, // Dancing with the Stars
            589, // Belly Up
            590, // No Leaves Left Behind
            591, // What's Yours Is Mine
            592, // The Root of the Problem
            593, // Killing Beasts Softly
            594, // It's a Trap
            595, // It's Hard Being Moogle
            604, // Refugee Raw
            605, // More than Meets the Eye
            606, // Necrologos: Olidious Separation
            607, // Revisiting Raimdelle
            608, // Up the Creek
            609, // Rope a 'Lope
            610, // Necrologos: The Beholders
            691, // Shaken, Not Stirred
            693, // Fueling the Flames
            695, // Moon in Rouge
            697, // Mushroom Gobblin'
            731, // Elemental Housekeeping
            733, // Rock My Wall
            735, // Can't Start a Fire
            737, // Baby, Light My Way
            771, // The Long and the Shortcrust
            773, // Blind Ambition
            775, // A Watery Web of Lies
            777, // The Truth Will Set You Free
        ],
        // T'mokkri
        [1000970] = [
            141, // Proly Hatchet
            142, // Hells Bells
            143, // Mending Fences
            147, // Stainless Chef
            148, // I, Gladius
            149, // Bring Me the Head Knife of Al'bedo Derssia
            153, // Farriers of Fortune
            154, // Anutha Spatha
            155, // Port of Call: Ul'dah
            159, // Smelt and Dealt
            160, // Awl or Nothing
            161, // Riveting Run
            165, // Unseamly Conditions
            166, // Claw Daddy
            167, // Hearth Maul
            171, // Time to Upgrade
            172, // Mors Dagger
            173, // Cleaving the Glim
            177, // The Naked Blade
            178, // Out on a Limb
            179, // Can You Spare a Dolabra
            183, // You Spin Me Round
            184, // Awl about Who You Know
            185, // Spice Cadet
            189, // A Weighty Question
            190, // A Mixed Message
            191, // File That under Whatever
            195, // I'd Rather Be Digging
            196, // No Refunds, Only Exchanges
            197, // You Stay on That Side
            201, // Ain't Got No Ingots
            202, // Skillet Labor
            203, // Eyes Bigger than the Plate
            207, // You've Got Mail
            208, // Headbangers' Thrall
            209, // Bronzed and Burnt
            213, // Waste Not, Want Not
            214, // All Ovo That
            215, // Greavous Losses
            219, // Cover Girl
            220, // Fashion Weak
            221, // Kiss the Pan (Good-bye)
            225, // Night Squawker
            226, // Get Me the Hard Stuff
            227, // 246 Kinds of Cheese
            231, // Ingot We Trust
            232, // A Leg to Stand On
            233, // Insistent Sallets
            237, // Eyes on a Hard Body
            238, // Aurochs Star
            239, // Romper Stomper
            243, // Very Slow Array
            244, // Hollow Hallmarks
            245, // Get Me the Usual
            249, // Liquid Persuasion
            250, // Everybody Cut Footloose
            251, // Distill and Know that I'm Right
            255, // Feasting the Night Away
            256, // Cobalt Aforethought
            257, // Some Dragoons Have All the Luck
            261, // Pork Is a Salty Food
            262, // Trout Fishing in Limsa
            263, // In Hot Water
            267, // Whip It
            268, // Jack of All Plates
            269, // A Real Fungi
            273, // Keep Your Powder Dry
            274, // Pretty Enough to Eat
            275, // Go Ahead and Dig In
            279, // Omelette's Be Friends
            280, // Shy Is the Oyster
            281, // A Total Nut Job
            285, // A Grape Idea
            286, // Brain Food
            287, // Feast of All Soles
            291, // Convalescence Precedes Essence
            292, // Cooking with Gas
            293, // Fever Pitch
            297, // Pretty as a Picture
            298, // Bloody Good Tart, This
            299, // True Grits
            303, // No More Dumpster Diving
            304, // Don't Turn Up Your Nose
            305, // Feeding Frenzy
            309, // Moving Up in the World
            310, // The Perks of Life at Sea
            311, // Made by Apple in Coerthas
            315, // Culture Club
            316, // The Egg Files
            317, // Bread in the Clouds
            507, // Bug Looks Like a Lady
            509, // Ain't Nobody Got Time for Rats
            510, // A Long and Winding Road
            511, // On the Lamb
            512, // Bug Bites
            526, // No Pain, No Grain
            527, // March of the Mandragoras
            528, // Red Stead Redemption
            529, // The Cure
            530, // Far from the Vine
            547, // A Tall Odor
            548, // The Sprite of Competition
            549, // Jackal and the Livestock
            550, // Shock Therapy
            551, // At the Gates
            553, // Fresh Meat
            555, // No Egg to Stand On
            574, // Sucked Dry
            575, // A Glamourous Life
            576, // First to Reave
            577, // Beasts of Bourbon
            578, // Field Testing
            579, // Barley Legal
            598, // Claw-struck
            599, // The Light Stripes
            600, // Another Egg Gone
            601, // Strand by Me
            602, // Man the Ore
            623, // Sol Survivors
            624, // Call Me Mating
            625, // Necrologos: Igneous Toil
            626, // Birds of a Feather
            627, // Out to Sea
            628, // Clearing Steer
            630, // Coeurl Scratch Fever
            631, // Walk Like a Mandragora
            632, // Necrologos: Igneous Moil
            633, // Bud Bait
            634, // Wonder Wine
            698, // The Palm in Your Hand
            699, // Nature Calls
            701, // Always Right
            738, // I Kidd You Not
            739, // That's What the Money Is For
            740, // The Midden Fair
            741, // Shell Game
            754, // Adventurers' Relish
            755, // Soup to Guts
            756, // A Kelping Hand
            757, // Poor Boys Eat Well
            758, // Cloud Cutlet
            759, // They Taste Just as Pretty
            760, // Yummy in the Tummy
            761, // Splendor in the Glass
            762, // Fish by Many Other Names
            763, // Just Add Water
            764, // Brain Candy
            765, // The Fertile Incandescent
            766, // My Own Private Shell
            767, // The Deepest Cut
            768, // The Moral of the Coral
            769, // Crab Life by the Horns
            778, // A Recipe for Disaster
            779, // Just Call Me Late for Dinner
            780, // Kitchen Nightmares No More
            781, // The Blue Period
        ],
        // Swygskyf
        [1001788] = [
            156, // Peddle to the Metal
            157, // I'm into Leather
            158, // Twice as Slice
            216, // Cook Intentions
            217, // Still the Best
            218, // Stadium Envy
            276, // Chew the Fat
            277, // Fisher of Men
            278, // The Bango Zango Diet
            547, // A Tall Odor
            548, // The Sprite of Competition
            549, // Jackal and the Livestock
            550, // Shock Therapy
            551, // At the Gates
            553, // Fresh Meat
            555, // No Egg to Stand On
            763, // Just Add Water
            765, // The Fertile Incandescent
        ],
        // Orwen
        [1001791] = [
            162, // Get a Little Bit Closer
            163, // Honest Ballast
            164, // Tools of the Trade
            222, // A Well-rounded Crew
            223, // A Firm Hand
            224, // Still Crazy After All These Years
            282, // Sweet Smell of Success
            283, // Rustic Repast
            284, // Flakes for Friends
            574, // Sucked Dry
            575, // A Glamourous Life
            576, // First to Reave
            577, // Beasts of Bourbon
            578, // Field Testing
            579, // Barley Legal
            766, // My Own Private Shell
            767, // The Deepest Cut
            768, // The Moral of the Coral
            769, // Crab Life by the Horns
        ],
        // Eustace
        [1001794] = [
            321, // Mercury Rising
            322, // One for the Books
            323, // Root Rush
            327, // On the Drip
            328, // Distill, My Heart
            329, // A Jawbreaking Weapon of Staggering Weight
            333, // Wand-full Tonight
            334, // Morning Glass of Ether
            335, // Using Your Arcane Powers for Fun and Profit
            339, // Shut Up and Take My Gil
            340, // Book and a Hard Place
            341, // Kiss of Life
            345, // Everything Is Impossible
            346, // Don't Forget to Take Your Meds
            347, // The Writing Is Not on the Wall
            351, // Automata for the People
            352, // Glazed and Confused
            353, // Sophomore Slump
            357, // Just Give Him a Serum
            358, // Riches' Brew
            359, // Stuck in the Moment
            363, // Alive and Unwell
            364, // The House Always Wins
            365, // Always Have an Exit Plan
            369, // A Patch-up Place
            370, // A Bile Business
            371, // Your Courtesy Wake-up Call
            375, // Sleepless in Silvertear
            376, // Quit Your Jib-jab
            377, // A Matter of Vital Importance
            381, // The Unmentionables
            382, // Trew Enough
            383, // Not Cool Enough
            387, // The Adventurer's New Coat
            388, // A Taste for Dalmaticae
            389, // Just for Kecks
            393, // Hat in Hand
            394, // Workplace Safety
            395, // Keep It under Wraps
            399, // Read the Fine Print
            400, // Don't Trew So Hard
            401, // Better Shroud than Sorry
            405, // New Shoes, New Me
            406, // Hitting Below the Belt
            407, // Doublet Jeopardy
            411, // Piling It On
            412, // I'll Be Your Wailer Today
            413, // He's Got Legs
            417, // By the Seat of the Pants
            418, // By the Short Hairs
            419, // Our Man in Ul'dah
            423, // Edmelle's Hair
            424, // Private Concerns
            425, // Crunching the Numbers
            429, // Cool to Be Southern
            430, // After the Smock-down
            431, // Party Animals
            435, // Full Moon Fever
            436, // Glad As a Hatter
            437, // Seeing It Through to the End
            441, // Copper and Robbers
            442, // Needful Rings
            443, // Arms for the Poor
            447, // Gods of Small Things
            448, // I Ram What I Ram
            449, // Let's Talk about Hex
            453, // All That Glitters
            454, // The Tusk at Hand
            455, // An Offer We Can't Refuse
            459, // Brothers in Arms
            460, // Forever 21K
            461, // Bad to the Bone
            465, // Perk of Fiction
            466, // King for a Day
            467, // You Burnt It, You Bought It
            471, // Love in the Time of Umbra
            472, // Thaumaturge Is Magic
            473, // All Booked Up
            477, // He Has His Quartz
            478, // One Man's Trash
            479, // A Little Bird Told Me
            483, // Actually, It's Loyalty
            484, // Lode It Up
            485, // Burning the Midnight Oil
            489, // Coral on My Mind
            490, // When We Were Blings
            491, // It's My Business to Know Things
            495, // Green and Bear It
            496, // Gold Is So Last Year
            497, // The Big Red
            513, // Road Rage
            515, // Nothing Compares to Shrews
            516, // Observe and Protect
            517, // Either Love Them or Kill Them
            518, // Berries Got Bucked
            532, // The Ore the Better
            533, // Needles to Say
            534, // Procession of the Noble
            535, // A Spoonful of Poison
            536, // Two Yalms Under
            557, // Reeking Havoc
            558, // Skin-deep
            559, // Beak to Beak
            560, // Out of Body, Out of Mind
            561, // Field of Beans
            563, // You Cannot Kill the Metal
            565, // From Ruins to Riches
            581, // The Enterprise of Undeath
            583, // A Simple Solution
            584, // Death with Indignity
            585, // Going for Goat
            586, // Earthbound
            587, // The Missing Merchant
            611, // Dead Men Lie
            612, // Drakes' Misfortune
            613, // Necrologos: Fluid Corruption
            614, // Flower Power
            615, // Run, Run Away
            714, // Rubble Bubble
            715, // Who Needs the Paperwork
            716, // The Primrose Path
            717, // Miners' Holiday
            718, // Vanishing Point
            719, // Do They Ore Don't They
            720, // Pipikkuli's Ship Comes In
            721, // I Don't Zinc It's a Coincidence
            722, // Break It Up
            723, // That's Why They Call It Fool's Gold
            724, // It Peiste to Listen
            725, // We Do This the Hard Way
            727, // Earth Sprites Are Easy
            729, // The Doom that Came to Belah'dia
        ],
        // Totonowa
        [1001796] = [
            336, // One for the Road
            337, // You Grow, Girl
            338, // Unbreak My Heart
            396, // Making Gloves Out of Nothing at All
            397, // Welcome to the Cotton Club
            398, // Dirt Cheap
            456, // Point of Honor
            457, // Gorgeous Gorget
            458, // Better Four Eyes than None
            557, // Reeking Havoc
            558, // Skin-deep
            559, // Beak to Beak
            560, // Out of Body, Out of Mind
            561, // Field of Beans
            563, // You Cannot Kill the Metal
            565, // From Ruins to Riches
            722, // Break It Up
            723, // That's Why They Call It Fool's Gold
            724, // It Peiste to Listen
            725, // We Do This the Hard Way
        ],
        // Poponagu
        [1001799] = [
            342, // There's Something about Bury
            343, // Sanity Points
            344, // Devil Take the Foremost
            402, // Pants Are Not Enough
            403, // Touch Me If You Can
            404, // A Drag of a Doublet
            462, // Sharp Words
            463, // Bad Guys Eat Brass
            464, // I Am a Rock
            581, // The Enterprise of Undeath
            583, // A Simple Solution
            584, // Death with Indignity
            585, // Going for Goat
            586, // Earthbound
            587, // The Missing Merchant
            727, // Earth Sprites Are Easy
            729, // The Doom that Came to Belah'dia
        ],
        // Muriaule
        [1001866] = [
            24, // Bowing Out
            25, // Got Your Back
            26, // Gridania's Got Talent
            30, // Leaving without Leave
            31, // A Sword in Hand
            32, // Compulsory Conjury
            84, // These Boots Are Made for Wailing
            85, // Sticking Their Necks Out
            86, // Tan Before the Ban
            90, // A Thorn in One's Hide
            91, // A Place to Call Helm
            92, // Throwing Down the Gauntlet
            501, // Water, Water Everywhere
            503, // Scourge of the Saplings
            504, // Stay for a Vile
            505, // Nutbreaker Suite
            506, // Picking Up the Piercers
            520, // A Vine Finer than Twine
            521, // River Raid
            522, // A Full Stomach
            523, // Wrangling Raptors
            524, // Compost with the Most
            674, // Waking Wood
            675, // East Bank Story
            676, // We Couldn't Start the Fire
            677, // Lovely Latex
            678, // Let the Sun Shine In
            679, // If a Tree Falls
            680, // Evil Seeds
            681, // No Bark, All Bite
        ],
        // Esmond
        [1002365] = [
            611, // Dead Men Lie
            612, // Drakes' Misfortune
            613, // Necrologos: Fluid Corruption
            614, // Flower Power
            615, // Run, Run Away
        ],
        // Aileen
        [1002367] = [
            630, // Coeurl Scratch Fever
            631, // Walk Like a Mandragora
            632, // Necrologos: Igneous Moil
            633, // Bud Bait
            634, // Wonder Wine
        ],
        // Cimeaurant
        [1002384] = [
            66, // Grippy When Wet
            67, // The Cold, Cold Ground
            68, // Bend It Like Durendaire
            126, // Springtime for Coerthas
            127, // Through a Glass Brightly
            128, // First They Came for the Heretics
            186, // Lending a Hand
            187, // Colder than Steel
            188, // Seemed Like the Thing to Get
            246, // War Is Tough on the Hands
            247, // Skillet to the Stars
            248, // I've Got You under My Skin
            306, // Winter of Our Discontent
            307, // Rise and Dine
            308, // Leek Soup for the Soul
            366, // Open Your Grimoire to Page 42
            367, // The Sting of Conscience
            368, // Going Nowhere Fast
            426, // The Wages of Sin
            427, // In over Your Head
            428, // A Leg Up on the Cold
            486, // Wear Your Patriotic Pin
            487, // Dead Can't Defang
            488, // Faith and Fashion
            636, // Now We're Even
            637, // Have a Nice Trip
            638, // Necrologos: Cinerary Rite
            639, // Meat and Bleat
            640, // Little Lost Lamb
            641, // Cower to the People
            703, // Spear of Heaven
            704, // Salad Days
            705, // Catch My Drift
            742, // Rocks for Brains
            743, // There Are No Good Answers
            744, // Eye for an Eye
            745, // Mythril Madness
            782, // The Perks of Politics
            783, // Hands off Our Fish
            784, // Fry Me a River
            785, // Gathering Light
        ],
        // Merthelin
        [1002397] = [
            617, // A Feast in the Forest
            618, // Black Market Down
            620, // Adamantoise Tears
            621, // Woodcross Busydeal
        ],
        // Rurubana
        [1002398] = [
            643, // Subduing the Subprime
            644, // Necrologos: Pale Oblation
            645, // Don't Forget to Cry
            646, // Circling the Ceruleum
            647, // Someone's in the Doghouse
        ],
        // Voilinaut
        [1002401] = [
            72, // A Winning Combo
            73, // The Turning Point
            74, // Ready for a Rematch
            132, // Foot Blues
            133, // Not So Alike in Dignity
            134, // It's Not a Job, It's a Calling
            192, // Kitchen Casualties
            193, // Get Me to the War on Time
            194, // Streamlining Operations
            252, // Metal Fatigue
            253, // Family Secrets
            254, // Employee Retention
            312, // Rolanberry Fields Forever
            313, // Good Eats in Ishgard
            314, // Pagan Pastries
            372, // No Accounting for Waste
            373, // Arcane Arts for Dummies
            374, // A Real Smooth Move
            432, // I'll Swap You
            433, // No Country for Cold Men
            434, // A Matter of Import
            492, // North Ore South
            493, // Tough Job Market
            494, // If You've Got It, Flaunt It
            649, // Necrologos: Whispers of the Gem
            650, // Got a Gut Feeling about This
            651, // You Look Good Enough to Eat
            652, // The Area's a Bit Sketchy
            653, // The Tropes of the Trade
            654, // Pets Are Family Too
            707, // Straight and Arrow
            709, // Sign of the Crimes
            747, // Brother in Arms
            748, // Breach and Build
            749, // Not Losing Our Heads This Time
            786, // Empire Builder
            787, // Laird of the Lakes
            788, // Make a Fish
            789, // Rationally Speaking
        ],
        // Graceful Song
        [1003888] = [
            324, // Met a Sticky End
            325, // Days of Chunder
            326, // The Bleat Is On
            330, // Gotta Bounce
            331, // Don't Be So Tallow
            332, // The Hexster Runoff
            384, // Hire in the Blood
            385, // Burn Me Up
            386, // Long Hair, Long Life
            390, // Wiggle Room
            391, // This Is Why You Can't Have Nice Things
            392, // Time for Acton
            444, // Hora at Me
            445, // Bad Bromance
            446, // Water of Life
            450, // A Ringing Success
            451, // Horn of Plenty
            452, // A Needle Is a Small Sword
            513, // Road Rage
            515, // Nothing Compares to Shrews
            516, // Observe and Protect
            517, // Either Love Them or Kill Them
            518, // Berries Got Bucked
            532, // The Ore the Better
            533, // Needles to Say
            534, // Procession of the Noble
            535, // A Spoonful of Poison
            536, // Two Yalms Under
            715, // Who Needs the Paperwork
            717, // Miners' Holiday
            719, // Do They Ore Don't They
            721, // I Don't Zinc It's a Coincidence
        ],
        // Wyrkholsk
        [1004342] = [
            144, // Axe Me Anything
            145, // The Unkindest Cut
            146, // Thank You for Your Business
            150, // Down on the Pharm
            151, // A Hit Job
            152, // As Above, Below
            204, // The Alloyed Truth
            205, // Don't Hit Me One More Time
            206, // Distill It Yourself
            210, // Rodents of Unusual Size
            211, // Strait Ain't the Gate
            212, // Get into Their Heads
            264, // What a Sap
            265, // Meat-lover's Special
            266, // It's Always Sunny in Vylbrand
            270, // Putting the Squeeze On
            271, // Butter Me Up
            272, // Fishy Revelations
            507, // Bug Looks Like a Lady
            510, // A Long and Winding Road
            511, // On the Lamb
            512, // Bug Bites
            526, // No Pain, No Grain
            528, // Red Stead Redemption
            529, // The Cure
            530, // Far from the Vine
            755, // Soup to Guts
            757, // Poor Boys Eat Well
            759, // They Taste Just as Pretty
            761, // Splendor in the Glass
        ],
        // Nahctahr
        [1004344] = [
            60, // The Lone Bowman
            61, // Live Freelance or Die
            62, // The Long Lance of the Law
            120, // The Hand that Bleeds
            121, // Slave to Fashion
            122, // Subordinate Clause
            180, // A Spy in the House of Love
            181, // Hard Knock Life
            182, // Don't Fear the Reaper
            240, // Skillet Scandal
            241, // Kitty Get Your Helm
            242, // They've Got Legs
            300, // Gegeruju Gets Down
            301, // Point Them with the Sticky End
            302, // Sole Survivor
            360, // The Write Stuff
            361, // Eye of the Beholder
            362, // Growing Is Knowing
            420, // Half Is the New Double
            421, // Put on Your Party Pants
            422, // Walk Softly and Carry a Big Halberd
            480, // Renascence Man
            481, // It's Only Love
            482, // Get the Green Stuff
            623, // Sol Survivors
            624, // Call Me Mating
            625, // Necrologos: Igneous Toil
            626, // Birds of a Feather
            627, // Out to Sea
            628, // Clearing Steer
            699, // Nature Calls
            701, // Always Right
            738, // I Kidd You Not
            739, // That's What the Money Is For
            740, // The Midden Fair
            741, // Shell Game
            778, // A Recipe for Disaster
            779, // Just Call Me Late for Dinner
            780, // Kitchen Nightmares No More
            781, // The Blue Period
        ],
        // Ourawann
        [1004347] = [
            598, // Claw-struck
            599, // The Light Stripes
            600, // Another Egg Gone
            601, // Strand by Me
            602, // Man the Ore
        ],
        // K'leytai
        [1004348] = [
            78, // Bow Down to Magic
            79, // Bowing to Greater Power
            80, // Incant Now, Think Later
            138, // Fuss in Boots
            139, // Tenderfoot Moments
            140, // Spelling Me Softly
            198, // Pop That Top
            199, // Talon Terrors
            200, // I Maul Right
            258, // Parasitic Win
            259, // Booty Call
            260, // Dealing with the Tough Stuff
            318, // Comfort Me with Mushrooms
            319, // Drinking to Your Health
            320, // Red Letter Day
            378, // Shut Up Already
            379, // Make Up Your Mind or Else
            380, // Not Taking No for an Answer
            438, // Big in Mor Dhona
            439, // And a Haircut Wouldn't Hurt
            440, // Bundle Up, It's Odd out There
            498, // Sew Not Doing This
            499, // Sweet Charity
            500, // Light in the Darkness
            656, // Turnabout's Fair Play
            657, // Necrologos: The Liminal Ones
            658, // Big, Bad Idea
            659, // Put Your Stomp on It
            660, // One of Our Naturalists Is Missing
            710, // Caught in the Long Grass
            711, // Topsy-turvy Time
            712, // Wonders Don't Cease, They're Discontinued
            713, // See How They Shine
            750, // Metal Has No Master
            751, // Crystal Mess
            752, // Sucker Seer
            753, // Hybrid Hypotheses
            791, // Awash in Evidence
            793, // Sleeper Creeper
        ],
        // Eugene
        [1004735] = [
            794, // Don't Touch Our Stuff
            795, // Victory Is Mine, Not Yours
            796, // The Creeps from Work
            797, // Throw the Book at Him
            806, // Of Mice and Demons
            807, // Wrong and Rite
            808, // Double Dose of Impin'
            809, // You're a Liar, Mansel Black
        ],
        // C'lafumyn
        [1004736] = [
            819, // It's Better (for You) Under the Sea
            821, // Twenty-nine Was the Cutoff
        ],
        // Cedrepierre
        [1004737] = [
            798, // Burn It Down
            799, // A Little Constructive Exorcism
            800, // Mortal Munchies
            801, // She's So Mean
            810, // Go with the Flow
            811, // Sylph Strands
            812, // Blinded by the Wight
            813, // A Real Wingnut
        ],
        // H'amneko
        [1004738] = [
            823, // This Is Going to Sting, A Lot
            824, // Creature Feature
            825, // It Goes with the Territory
            826, // Dead Man Walking
        ],
        // Kikiri
        [1004739] = [
            802, // A Cold-blooded Business
            803, // Watch Me If You Can
            804, // Blood in the Water
            805, // The Burning Reason
            814, // Don't Tear Down This Wall
            815, // Food for Thought
            816, // The Third Prize Is that You're Slain
            817, // A Heart Aflame
        ],
        // Blue Herring
        [1004740] = [
            829, // And Then There Were None
            831, // Grapevine of Wrath
        ],
        // Haisie
        [1007068] = [
            833, // Fanning the Flames
            834, // Fishing off the Company Pier
            835, // Mad about You
            838, // Feathered Foes
            839, // Just Making an Observation
            840, // Roast Lamb with Mint and Hellfire
            843, // The Baddest Brigade in Town
            844, // Dress for Aggress
            845, // Pick Your Poison
        ],
        // Lodille
        [1007069] = [
            848, // Someone's Got a Big Mouth
            849, // An Imp Mobile
            850, // Papal Dispensation
            853, // Yellow Is the New Black
            854, // Talk to My Voidsent
            855, // The Bloodhounds of Coerthas
            858, // You Dropped Something
            859, // No Big Whoop
            860, // If You Put It That Way
        ],
        // Eidhart
        [1007070] = [
            863, // One Big Problem Solved
            864, // Kill the Messenger
            865, // Go Home to Mama
            868, // The Awry Salvages
            869, // Amateur Hour
            870, // Get off Our Lake
            873, // Who Writes History
            874, // Science Shindig
            875, // The Museum Is Closed
        ],
        // Eloin
        [1011208] = [
            878, // The Unexpected Tourist
            879, // Don't Come Back
            880, // Necrologos: Of Sallow Vizards
            881, // We're So Above This (L)
            882, // The Second Coming of Yak (L)
            883, // Necrologos: Of Sallow Vizards (L)
            884, // Quit Loafing Around
            885, // Feathers Unsullied
            886, // Jailbird Break
            887, // Last Priest Profaned (L)
            888, // Scavenger Hunt (L)
            889, // Return to Sender (L)
            890, // Show Your Work
            891, // Puppet Show
            892, // Cry Home
            893, // Family Comes First (L)
            894, // More Than One Way (L)
            895, // Hello, Cousin (L)
            896, // Two Birds, One Culling
            897, // Don't Eat the Shrooms
            898, // Marl-ementary Procedure
            899, // Just Washed (L)
            900, // Needs More Fervor (L)
            901, // Nobody Can Farm Marl, but Marl (L)
            902, // Dance, Magic Dance
            903, // Necrologos: His Treasure Forhelen
            904, // Whither the Morbol Goeth
            905, // Goblin Up Sharlayan (L)
            906, // Necrologos: His Treasure Forhelen (L)
            907, // Bareback Riding (L)
            908, // Splinter in the Sewers
            909, // So You Think You Can Lance?
            910, // Almost as Fun as Slingshotting Birds
            911, // The Lumber of Their Discontent (L)
            912, // Sticks and Stones (L)
            913, // Living Bow to Mouth (L)
            914, // Do You Even String Bow
            915, // Landing the Big One
            916, // A Reward Fitting of the Faithful
            917, // Win One Bow, Get Three Free (L)
            918, // Fishing for Profits (L)
            919, // Just Rewards for Just Devotion (L)
            920, // License to Heal
            921, // The Darkest Hearth
            922, // Walking on Pins and Needles
            923, // Purified Polyrhythm (L)
            924, // Fruit of the Loom (L)
            925, // Like Lemon on a Lumbercut (L)
            926, // The Long Armillae of the Law
            927, // Don't Ask Wyvern
            928, // Aim to Please
            929, // Wooden Ambitions (L)
            930, // A Sky Pirate's Life for Me (L)
            931, // To Protect My City, I Must Wear a Mask (L)
            932, // Birch, Please
            933, // Anatomy of a Drill Bit
            934, // Hold on Adamantite
            935, // Built This City on Blocks and Soul (L)
            936, // Pulling Them to the Grind (L)
            937, // Spears for Stone Vigilantes (L)
            938, // Pummeling Abroad
            939, // From Mud to Mourning
            940, // Glorified Hole-punchers
            941, // The Style of the Time (L)
            942, // These Boots Are Made for Hawkin' (L)
            943, // Treat Them with Kid Gloves (L)
            944, // You Could Say It's a Moving Target
            945, // Maybe He's a Lion
            946, // (Don't) Love the Skin You're In
            947, // They Call It Bloody Mary (L)
            948, // The Wyvern of It (L)
            949, // Wrist Apart (L)
            950, // Overall, We Blend In
            951, // Tally Ho, Chocobo
            952, // Dragoon Drop Rate
            953, // Eviction Notice (L)
            954, // Hunting Heretics (L)
            955, // Exploiting the Adroit (L)
            956, // Don't Sweat the Small Fry
            957, // I Need Your Glove Tonight
            958, // Trainin' the Neck
            959, // It's All in the Wrists (L)
            960, // Halonic Drake Handlers (L)
            961, // Training Is Only Skintight (L)
            962, // Starting Off on the Wrong Foot
            963, // Bar of the Bannermen
            964, // It Will Knock Your Socks Off
            965, // Raising the Dragoons (L)
            966, // Do My Little Turn on the Stonewalk (L)
            967, // On My Own Two Feet (L)
            968, // Barring the Gates to Foundation
            969, // Punching Your Way to Success
            970, // With Bearings Straight
            971, // Starting Young (L)
            972, // Foreign Exchange (L)
            973, // Bearing the Brunt (L)
            974, // It's All about Execution
            975, // Freight and Barrel
            976, // Cautionary Cutlery
            977, // Too Big to Miss (L)
            978, // Stepping on My Heart with Stilettos (L)
            979, // Saw, Shank, and Redemption (L)
            980, // I Could Feel That from Here
            981, // I Saw the Pine
            982, // Keep Up with the Mechanics
            983, // Tensions in Creasing (L)
            984, // I Came, I Sawed, I Conquered (L)
            985, // Unconventional Weaponry (L)
            986, // Unbreaker
            987, // Diamond Sawdust
            988, // Spirituality Inspector
            989, // Attack on Titanium (L)
            990, // I'm a Lumberjack and I'm Okay (L)
            991, // The Clamor for Hammers (L)
            992, // Through Thick and Thin
            993, // Winter Weather Conditions
            994, // Swords for Plowshares
            995, // Piercing Eyes Deserve Piercing Shafts (L)
            996, // The Nightsoil Is Dark and Full of Terrors (L)
            997, // Negative, They Are Meat Popsicles (L)
            998, // Hauberk and No Play
            999, // Rivets Run through It
            1000, // Don't Scuttle with Scuta
            1001, // Knights without Armor (L)
            1002, // A Riveting Revival (L)
            1003, // Shielded by Bureaucracy (L)
            1004, // Let Faith Light the Way
            1005, // The Cut Alembical Cord
            1006, // Pan That Laid the Golden Egg
            1007, // Fifty Shields of Blades (L)
            1008, // Sheer Distill Power (L)
            1009, // Skillet with Fire (L)
            1010, // As the Bolt Flies
            1011, // Someone Put Dung in My Helmet
            1012, // Sometimes the South Wins
            1013, // Heavy Metal Banned (L)
            1014, // Rage against the Scream (L)
            1015, // The Thriller of Autumn (L)
            1016, // A Squire to Inspire
            1017, // A Halonic Masquerade
            1018, // Belle of the Brawl
            1019, // All's Fair in Highborn Assassination (L)
            1020, // Why I Wear a Mask (L)
            1021, // Shouldering the Shut-ins (L)
            1022, // Sir, Dost Thou Even Heft
            1023, // Look Before You Leap
            1024, // The Mast Chance
            1025, // Men in Adamantite (L)
            1026, // Patience, Young Grasshopper (L)
            1027, // The Rose and the Riveter (L)
            1028, // Little Orphan Candy
            1029, // The Next to Last Supper
            1030, // The Aroma of Faith
            1031, // Confections of Confession (L)
            1032, // Nostalgia through the Stomach (L)
            1033, // Soup's On (L)
            1034, // Such a Butter Face
            1035, // Loving That Muffin Top
            1036, // Persona non Gratin
            1037, // No Margarine of Error (L)
            1038, // Muffin of the Morn (L)
            1039, // Recipe for Disaster (L)
            1040, // The Nutcracker's Sweets
            1041, // Breakfast of Champions
            1042, // Old Victories, New Tastes
            1043, // Time for a Midnight Snack (L)
            1044, // Emerald Soup for the Soul (L)
            1045, // The Eats of Authenticity (L)
            1046, // Saucy for a Suitor
            1047, // It Goes Down Smoothly
            1048, // Persuasion of a Higher Power
            1049, // Saved by the Sauce (L)
            1050, // Quenching the Flame (L)
            1051, // Loaves and Fishes (L)
            1052, // Let's Not Get Sappy
            1053, // Soup That Eats Like a Knight
            1054, // Don't Let It Fall Apart
            1055, // Luxury Spillover (L)
            1056, // Like Ma Used to Make (L)
            1057, // Better Come Back with a Sandwich (L)
            1058, // The Mustache Suits Him
            1059, // Summoning for Dummies
            1060, // Forged from the Void
            1061, // Forgery of Convenience (L)
            1062, // Summoning the Courage to Be Different (L)
            1063, // Dodging the Draft (L)
            1064, // Can't Sleep, Inquisitors Will Eat Me
            1065, // Steeling the Knife, Steeling the Mind
            1066, // Consecrating Congregation
            1067, // Allow No Fallacies (L)
            1068, // Surgical Substitution (L)
            1069, // Curbing the Contagion (L)
            1070, // Adhesive of Antipathy
            1071, // Tomes Roam on the Range
            1072, // Warding Off Temptation
            1073, // It's Gonna Grow Back (L)
            1074, // Field Trip to the Unknown (L)
            1075, // The Garden of Arcane Delights (L)
            1076, // Cleansing the Wicked Humours
            1077, // Volunteering with Staff
            1078, // Rolling on Initiative
            1079, // Washing Away the Sins (L)
            1080, // Scripture Is the Best Medicine (L)
            1081, // Darkly Dreaming Dexterity (L)
            1082, // Filling in the Blanks
            1083, // There Was a Late Fee
            1084, // The Grave of Hemlock Groves
            1085, // Ink into Antiquity (L)
            1086, // A Gate Arcane Is Dragon's Bane (L)
            1087, // Dappling the Highlands (L)
            1088, // Pride Up in Smoke
            1089, // Protecting the Foundation
            1090, // Ribbon of Remembrance
            1091, // Desperate for Diversionaries (L)
            1092, // Curb the Gnawing Feeling (L)
            1093, // The Road Was a Ribbon of Moonlight (L)
            1094, // What Not to Wear
            1095, // Fashion Patrol
            1096, // An Account of My Boots
            1097, // Appeal of Foreign Apparel (L)
            1098, // Dress Code Violation (L)
            1099, // Soot in My Hair and Scars on My Feet (L)
            1100, // Clothing the Naked Truth
            1101, // Storm upon Bald Mountain
            1102, // Finger on the Pulse
            1103, // When in Robes (L)
            1104, // Abrupt Apprentices (L)
            1105, // Chirurgeon Hand in Glove (L)
            1106, // Healing with Flair
            1107, // Where the Dragonflies, the Net Catches
            1108, // Investing in the Future
            1109, // Pants Fit for Battle (L)
            1110, // To Kill a Dragon on Nameday (L)
            1111, // Maids of Honor (L)
            1112, // Felt for the Fallen
            1113, // He Wears the Pants
            1114, // The Hat List
            1115, // Blinded Veil of Vigilance (L)
            1116, // Pom Hemlock (L)
            1117, // Knight Incognito (L)
            1118, // The Goggles, They Do Naught
            1119, // Not on My Table
            1120, // Halonic Hermeneutics
            1121, // Peril Never Wore Safety Goggles (L)
            1122, // Heinz's Dilemma (L)
            1123, // Transposing Theology (L)
            1124, // Sense of Entitlement
            1125, // High Above Me, She Sews Lovely
            1126, // Sky Is the Limit
            1127, // Charting the Trends (L)
            1128, // Old-school Spooling (L)
            1129, // Hulls of Broken Dreams (L)
            1130, // The Unfortunate Retirony
            1131, // Citizen's Arrest
            1132, // The Monuments Mages
            1133, // Life Ends at Retirement (L)
            1134, // Watchers within the Walls (L)
            1135, // Deal with It (L)
            1136, // Needs More Prayerbell
            1137, // The Grander Temple
            1138, // Appeasing the Astromancer
            1139, // With a Noise That Reaches Heaven (L)
            1140, // Man with a Dragon Earring (L)
            1141, // Silver Bar of Upcycling (L)
            1142, // Keeping Claw and Order
            1143, // Embroiling Embroidery
            1144, // A Halo for Her Head
            1145, // Ring of Reciprocity (L)
            1146, // The Lovely Hands of Haillenarte (L)
            1147, // It's the Circlet of Life (L)
            1148, // Does This Look Infected?
            1149, // Breathe Deeply
            1150, // Warm in Their Beds
            1151, // Secondhand Smoke Screen (L)
            1152, // A Crown for Every Head (L)
            1153, // No Rest for the Thicket (L)
            1154, // Ladies and Gentians
            1155, // The Bitter, the Better
            1156, // Six Hours in a Leaky Boat
            1157, // Coat the Harm (L)
            1158, // The Sour Patch Grids (L)
            1159, // Hot Tub Clime Machine (L)
            1160, // Your Mother Is Medicine and Your Father Is the Wild
            1161, // Dandelion Don't Tell No Lies
            1162, // Chewed Up and Snuffed Out
            1163, // Exotic Remedies (L)
            1164, // Sacrilege Neutralized (L)
            1165, // Hybrid Theories (L)
            1166, // Heart to Heart
            1167, // Bold and Blue
            1168, // The Anointing of the Dead
            1169, // Putting on Airs (L)
            1170, // These Colors Run Not (L)
            1171, // Nectar of the Goddess (L)
            1172, // Watching the Western Wall
            1173, // Bleeding Out
            1174, // A Taste of Their Own Medicine
            1175, // Blending In (L)
            1176, // Mending Wings (L)
            1177, // Thank You for Smoke Screening (L)
            1178, // Taken for Granite
            1179, // The Road to Pilgrimage
            1180, // The Merits of Upcycling
            1181, // The Basics of Forgery (L)
            1182, // For Vares Beyond Compare (L)
            1183, // Breaking Beacons (L)
            1184, // Permit for Destruction of Religious Property
            1185, // I'll Show You My Battle Shards
            1186, // Halone's Jewelry Box
            1187, // Rose Never Lets Go (L)
            1188, // Forging Lance Base (L)
            1189, // Polished till They Shine (L)
            1190, // From Creepers to Squatters
            1191, // Dreams of War, Dreams of Liars, Dreams of Dragon Fire
            1192, // Talk about Boundaries
            1193, // All of These Bases Belong to Us (L)
            1194, // There's Sand in My Water (L)
            1195, // History Needs Some Revisions (L)
            1196, // Fake-icite
            1197, // What Goes Up
            1198, // Mortarin'
            1199, // Fool Me Once (L)
            1200, // The Puppets of War (L)
            1201, // A Spire for New Heights (L)
            1202, // Sticking It Out
            1203, // Crystal Chronicles
            1204, // Dragonproofing
            1205, // Pommeling the Enemy (L)
            1206, // Not So Crystal Clear (L)
            1207, // Sharlayan Sympathizers (L)
            1208, // Please Halone, Tell Me I'm Still Asleep
            1209, // The Voice of the Fury
            1210, // Dine or Spine
            1211, // Snipped for Spirituality (L)
            1212, // A Whole Lot of Nope (L)
            1213, // Lurchin' from Urchins (L)
            1214, // Loose Lips Heal (Broken) Hips
            1215, // Fish Oils and Forgotten Spoils
            1216, // Bounty of Sky, Bounty of Earth
            1217, // Prayer and Prejudice (L)
            1218, // Valuing the Vintage (L)
            1219, // Eating Like the Natives (L)
            1220, // Analysis of Paralysis
            1221, // The Aquariums of Ishgard
            1222, // Bearing of the Blue
            1223, // Send a Feeling to My Spine (L)
            1224, // A Win-win Situation (L)
            1225, // Blue of Sky and Sea (L)
            1226, // Dining with Dravanians
            1227, // Solo Out the Bolo
            1228, // Pipira Pirouette
            1229, // Hundred Fins for a Hundred Wings (L)
            1230, // Sucking on Face (L)
            1231, // Plus One or Two or Three (L)
            1232, // Spew Forth and Spawn
            1233, // They Call It the Kissing Disease
            1234, // Warmer than Wine
            1235, // Unleash the Hydro Cannons (L)
            1236, // Angling for Ailments (L)
            1237, // What Does Not Break Us, Devours Us (L)
        ],
        // Keltraeng
        [1018997] = [
            1238, // Walk the Walk
            1239, // Reeling for Rods
            1240, // Beech, Please
            1241, // Standing on Ceremony
            1242, // Composition
            1243, // Wood That You Could
            1244, // Pinewheel
            1245, // O Pine
            1246, // Run Before They Walk
            1247, // Everybody's Heard about the 'Berd
            1248, // The Ear Is the Way to the Heart
            1249, // Spare a Rod and Spoil the Fishers
            1250, // Putting Your Line on the Neck
            1251, // Zelkova, My Love
            1252, // With a Bow on Top
            1253, // Vested Interest
            1254, // Hide to Go Seek
            1255, // Fitting In
            1256, // Weathering Heights
            1257, // Off the Cuff
            1258, // Looking for Glove
            1259, // Try Tricorne Again
            1260, // Shoe on the Other Foot
            1261, // Tiger in the Sack
            1262, // A Stitch in Time
            1263, // Shrug It On
            1264, // Security Breeches
            1265, // Brace Yourselves
            1266, // Thick and Thin
            1267, // If the Shoe Fits
            1268, // Have Blade, Will Travel
            1269, // A Knack for Nicking
            1270, // High Steal
            1271, // Crisscrossing
            1272, // Hammer Time
            1273, // File under Dull
            1274, // Killer Cutlery
            1275, // Meddle in Metal
            1276, // And My Axe
            1277, // Sea-saw
            1278, // Renting Mortality
            1279, // The Bigger the Blade
            1280, // Hammer and Sails
            1281, // Ingot to Wing It
            1282, // Fire for Hire
            1283, // Mail It In
            1284, // Alembic Medals
            1285, // Setting the Stage
            1286, // Shielded Life
            1287, // The Gauntlet Is Cast
            1288, // Ore for Me
            1289, // Greaving
            1290, // Home Cooking
            1291, // En Garde and on Guard
            1292, // Art Imitates Life
            1293, // Smells of Rich Tama-hagane
            1294, // Sweeping the Legs
            1295, // See Shields by the Sea Shore
            1296, // Spoony Is the Bard
            1297, // Heads Will Roll
            1298, // Oh No Udon
            1299, // Loquacious
            1300, // All You Can Stomach
            1301, // Soup for the Soul
            1302, // Hunger Is No Game
            1303, // The Frier Never Lies
            1304, // Sweet Kiss of Death
            1305, // A Shorlonging for the Familiar
            1306, // Souper
            1307, // No Othard Choice
            1308, // Persimmony Snicket
            1309, // West Meats East
            1310, // Fits to a Tea
            1311, // Fish Box
            1312, // Herky Jerky
            1313, // Whinier than the Sword
            1314, // Spellbound
            1315, // Magic Beans
            1316, // Official Strategy Guide
            1317, // Scroll Down
            1318, // Materia Worth
            1319, // The Dotted Line
            1320, // Rumor Has It
            1321, // Asking for a Friend
            1322, // Edge of the Arcane
            1323, // Spell-rebound
            1324, // Let Loose the Juice
            1325, // Pep-stepper
            1326, // Ultimate Official Strategy Guide
            1327, // Making Your Mark
            1328, // Modest Beginnings
            1329, // What Guides Want
            1330, // Proper Props
            1331, // Duress Rehearsal
            1332, // Skills on Display
            1333, // Getting a Leg Up
            1334, // Apparent Apparel
            1335, // Say Yes to Formal Dress
            1336, // Of Great Import
            1337, // Who War It Better
            1338, // Don't Sweat the Role
            1339, // To the Tops
            1340, // Brimming with Confidence
            1341, // One Winged Angle
            1342, // Cap It Off
            1343, // Play It by Ear
            1344, // One Ring Circus
            1345, // Wants and Needles
            1346, // Chain of Command
            1347, // Bracelet for Impact
            1348, // If I'd a Koppranickel for Every Time...
            1349, // Cutting Deals
            1350, // Needle in a Hingan Stack
            1351, // Hair-raising Action
            1352, // Best-laid Planispheres
            1353, // Put the Metal to the Peddle
            1354, // Ring in the New
            1355, // Speak Softly and Carry a Metal Rod
            1356, // Untucked
            1357, // Choker in the Clutch
            1358, // Rhalgr Wood Too
            1359, // Thank Heavenspillar
            1360, // Barking Up the Right Tree
            1361, // Pining
            1362, // Coral-lation
            1363, // Penned-up Frustration
            1364, // Bamboozled
            1365, // Timbr
            1366, // Flowers for Algae Run
            1367, // Craic of Dawn
            1368, // Hypocritic Oath
            1369, // Leaves Much to Be Desired
            1370, // Nunh the Wiser
            1371, // Last of the Mhigans
            1372, // Garden Variety
            1373, // Cermet Breaker
            1374, // Set in Stone
            1375, // Axe to Grind
            1376, // Ready Ore Not
            1377, // Hit Rock Bottom
            1378, // Coral-lary
            1379, // Simply Marble Us
            1380, // Scraptacular
            1381, // No Stone Unturned
            1382, // Bead 'Em Up
            1383, // O Say Can You Rock
            1384, // Dunes of Our Lives
            1385, // Mine All Mine
            1386, // Adios, Ala Mhigo
            1387, // The Ores Have It
            1388, // Slow Wash, Rapids Jumper
            1389, // If a Leaf Falls in the Water
            1390, // There Can Be Only One
            1391, // Lighter Wallets
            1392, // In a Pickle
            1393, // Perhaps Not-so-common
            1394, // Catfish Scheme
            1395, // Curtains for Pleco
            1396, // Marooned Minnow
            1397, // Peculiar De-light
            1398, // Step by Steppe
            1399, // Unbeliebubble
            1400, // Blood in the Water
            1401, // To the Teeth
            1402, // Pre-octopied
        ],
        // Eirikur
        [1027847] = [
            1403, // Built to Last
            1404, // Just Starting Out
            1405, // Playing the Market
            1406, // A Stronger Offense
            1407, // Taking Aim
            1408, // Understaffed
            1409, // Patient Patients
            1410, // Ground to a Halt
            1411, // Horde of the Rings
            1412, // The Right Tool for the Job
            1413, // Sleep on It
            1414, // A Miss and a Hit
            1415, // Off to a Good Staff
            1416, // Kindling the Flame
            1417, // Safety First
            1418, // Ware and Chair
            1419, // Girding for Glory
            1420, // Band Substances
            1421, // Breeches of Trust
            1422, // Glove Me Tender
            1423, // A Slippery Slope
            1424, // Peace in Rest
            1425, // A Heady Endeavor
            1426, // At Your Neck and Call
            1427, // Protecting the Nuts
            1428, // If I Could Walk a Thousand Malms
            1429, // Strike True
            1430, // Fit for a Friend
            1431, // Into the Storm
            1432, // A Shoe In
            1433, // The Gold Experience
            1434, // Fire Sale
            1435, // Here Comes the Hammer
            1436, // Selective Logging
            1437, // Heavy Hitter
            1438, // Enlistment Highs
            1439, // Instruments of Distraction
            1440, // Halfhearted Effort
            1441, // Nip It in the Bud
            1442, // Dodge Once, Cut Twice
            1443, // Idol Hands
            1444, // Cooking for the Future
            1445, // Bae Blade
            1446, // Under the Fool Moon
            1447, // Keeping Loyalty
            1448, // Shielding the Realm
            1449, // Time to Fry
            1450, // A Head of Demand
            1451, // Scheduled Maintenance
            1452, // Hedging Bets
            1453, // Wrapped Knuckles
            1454, // Catching an Earful
            1455, // A New Regular
            1456, // The Proper Precautions
            1457, // No Scope
            1458, // Signed, Shield, Delivered
            1459, // A Budding Business
            1460, // Trial and Error
            1461, // One Foot Forward
            1462, // Shield to Shield
            1463, // Slippery Service
            1464, // Meet for Meat
            1465, // Cure for What Ails
            1466, // His Dark Utensils
            1467, // Soup for the Soldier
            1468, // Sweet Tooth
            1469, // Can't Eat Just One
            1470, // One Last Meal
            1471, // Mixology
            1472, // On a Full Stomach
            1473, // A Good Omen
            1474, // Teetotally
            1475, // Super Dark Times
            1476, // A Happy End
            1477, // A Cookie for Your Troubles
            1478, // Another Man's Ink
            1479, // Keeping Magic Alive
            1480, // A Time for Peace
            1481, // Make It Bigger
            1482, // An Eye for Healing
            1483, // Making Ends Meet
            1484, // Amaro Kart
            1485, // Conserving Combat
            1486, // 5-bell Energy
            1487, // Growing Up
            1488, // A Greater Grimoire
            1489, // Crafty Concoctions
            1490, // A Labor of Love
            1491, // Supreme Official Strategy Guide
            1492, // Mindful Medicine
            1493, // Flax Wax
            1494, // Lovely Leggings
            1495, // Turban in Training
            1496, // Suits You
            1497, // Legs for Days
            1498, // Hair Do No Harm
            1499, // A Tender Table
            1500, // Hunting Season
            1501, // Gloves Come in Handy
            1502, // All-purpose Overgarments
            1503, // The Hunt Continues
            1504, // Something in My Eye
            1505, // A Job Well Done
            1506, // A Turban for the Ages
            1507, // Healing Headwear
            1508, // Whetstones for the Workers
            1509, // Satisfactory Sewing
            1510, // You're My Wonderhall
            1511, // Slimming Down
            1512, // Neck on the Line
            1513, // Gentleman Donor
            1514, // Copious Crystal Cannons
            1515, // Hot Rod
            1516, // Unsung Generosity
            1517, // Prophet of Profit
            1518, // Birth Ring
            1519, // A Magnanimous Refrain
            1520, // Bulking Up
            1521, // A Beneficent Elegy
            1522, // Wrap Those Wrists
            1523, // Sought-after Spices
            1524, // Medicinal Herbs
            1525, // Home Is Where the Heart Is
            1526, // Packs a Punch
            1527, // Home Is Where the Heart Isn't
            1528, // Seeds for the Sick
            1529, // Pest Problems
            1530, // Fresh off the Boat
            1531, // Dream a Little Dream
            1532, // Spiritual Ventures
            1533, // Culinary Concepts
            1534, // The Only Cure
            1535, // Good Business
            1536, // The Sweetest Syrup
            1537, // Big Business
            1538, // Jewelry for All
            1539, // The Search for Slag
            1540, // Barmy for Ballistas
            1541, // Secret Stones
            1542, // New Necklaces
            1543, // Crystallized Revenge
            1544, // Rocks from Rak'tika
            1545, // Jewels for Jewelry
            1546, // Knowledge Is Power
            1547, // Road to Recovery
            1548, // Jewelry Is Forever
            1549, // The Magic of Mining
            1550, // Lakeland's Legacy
            1551, // Back Stronger
            1552, // Crystal Meds
            1553, // Eco-Warrior of Light
            1554, // Needs More Egg
            1555, // The Source of the Problem
            1556, // Jelly Salad
            1557, // Full of Fish
            1558, // Aetherquake
            1559, // Faerie Fish
            1560, // The Bride Aquatic
            1561, // A Cherry-red Herring
            1562, // Crab Corps
            1563, // Magic Mushrooms
            1564, // Fish for Days
            1565, // Short-term Gains
            1566, // Deep-sea Diving
            1567, // A Feast for the Senses
        ],
        // Grigge
        [1037263] = [
            1568, // Timber of Tenkonto
            1569, // A Real Grind
            1570, // Earring Awakening
            1571, // A Wristy Experiment
            1572, // A Better Conductor
            1573, // In Rod We Trust
            1574, // An A-prop-riate Request
            1575, // Spinning the Time Away
            1576, // Annals of the Empire II
            1577, // An Integral Reward
            1578, // To Delight a Dancer
            1579, // Archon Denied
            1580, // History of the Hrothgar
            1581, // Archon of His Eye
            1582, // Records of the Republic
            1583, // In Pursuit of Panaloaf
            1584, // Mangalomania
            1585, // Pruned to Perfection
            1586, // Annals of the Empire I
            1587, // Plying with Precision
            1588, // Haste for High Durium
            1589, // The Armoire Is Open
            1590, // Ace of Gloves
            1591, // The Incomplete Costume
            1592, // Armoire Aftercare
            1593, // Once and for Alchemy
            1594, // Heading toward Bankruptcy
            1595, // In-kweh-dible Cooking
            1596, // A Gift of Gloves
            1597, // Additions to the Armoire
            1598, // Awarding Academic Excellence
            1599, // Workplace Workout
            1600, // The Sage's Successor
            1601, // Pewter-hewn Punishment
            1602, // Gold Rush Order
            1603, // Sage with the Golden Earrings
            1604, // To Fight at Her Side
            1605, // The Needle That Binds
            1606, // Planisphere to Paper
            1607, // Star Athletes
            1608, // Hell on Leather
            1609, // Running up the Tabi
            1610, // Boot Legs
            1611, // Scouting Talent
            1612, // Battered Books
            1613, // Loyal Turncoat
            1614, // Grips of Fear
            1615, // Loving Soles
            1616, // Generous Soles
            1617, // For What Was Gleaned
            1618, // Heavy Armoire
            1619, // Helping Handwear
            1620, // Hot Heads
            1621, // Color Coated
            1622, // A Polished Purchase
            1623, // Turban Sprawl
            1624, // Lightening Up
            1625, // Lifetime of Gleaning
            1626, // Skill Cap
            1627, // A Better Bottom Line
            1628, // Wishful Inking
            1629, // Nearly Bare
            1630, // Luncheon Bound
            1631, // Body over Mind
            1632, // Rebuilding to Code
            1633, // Liquid Competence
            1634, // Nearly There
            1635, // Practical Command
            1636, // Technically Still Magic
            1637, // Mindful Study
            1638, // Salt of the North
            1639, // Topping Up the Pot
            1640, // Bobbing for Compliments
            1641, // At Any Temperature
            1642, // Imperial Palate
            1643, // A Stickler for Carrots
            1644, // A Historical Flavor
            1645, // Comfort Food
            1646, // Blast from the Pasta
            1647, // The Mountain Steeped
            1648, // Paper Minds
            1649, // A Balanced Diet
            1650, // Bug Report
            1651, // Don't Have a Yakow
            1652, // Soup for the Stars
            1653, // Explosive Palms
            1654, // Beet It
            1655, // Into the Pines
            1656, // Through the Fires and Flames
            1657, // Tea Off
            1658, // When Size Matters
            1659, // In Case of Emergency
            1660, // A Natural Style
            1661, // Wood Envy
            1662, // Poisonous Palms
            1663, // One Man's Rock
            1664, // Explosive Progress
            1665, // An Unstable Foundation
            1666, // The Gall on That Giant
            1667, // Just in Lime
            1668, // To Boldly Gall
            1669, // Training Up
            1670, // A Rock After My Own Heart
            1671, // March for Magitek
            1672, // Stone Cold
            1673, // Sand in My Boots
            1674, // Enriching the Soil
            1675, // Rocks of a Feather
            1676, // The Final Touch
            1677, // Reactionary Reactors
            1678, // Fish for Thought
            1679, // Crabs for the Crabby
            1680, // Water Works
            1681, // Fishing for the Future
            1682, // Making Waves
            1683, // What Would You Do for a Pickle
            1684, // Fungi of the Firmament
            1685, // Simple as Salt
            1686, // Plumbing the Past
            1687, // What's in the Air
        ],
        // Malihali
        [1048390] = [
            1688, // Hull Lotta Damage
            1689, // Yimepi's Country Charms
            1690, // Wood You Be Quiet
            1691, // The Wing's Wings
            1692, // Turali Quality
            1693, // Lament of the Lazylump
            1694, // Bow Out
            1695, // Weaving a Path
            1696, // Spear Pressure
            1697, // No Greater Treasure
            1698, // Always Be Prepaired
            1699, // Paring Is Caring
            1700, // Ruthenium Supremium
            1701, // Axes to the Maxes
            1702, // Maintaining the Maintainers
            1703, // Dagger Swagger
            1704, // Bladewinner
            1705, // Maul Me
            1706, // Ceremonial Teeth
            1707, // Awl Dreams Come True
            1708, // Don't Bore Me, Ore Me
            1709, // Shielding My Students
            1710, // Brace for More Vambraces
            1711, // Forgiveness for My Shins
            1712, // Metal with Mettle
            1713, // Odd Instruments
            1714, // Don't Ask about the Rivets
            1715, // Backing up My Words
            1716, // A Hand for a Deckhand
            1717, // Essays on Equipment
            1718, // On Board for Lar
            1719, // Pendulums of Our Own
            1720, // Guaranteed Gem
            1721, // Fan of the Foreign
            1722, // Shiny and Good
            1723, // Sew Excited
            1724, // Orders Anonymous
            1725, // Ringing Gratitude
            1726, // The Right Rod
            1727, // Mask Maker
            1728, // Tenets of Tanning
            1729, // The Perfect Accessory
            1730, // Freezing Fingers
            1731, // Dreams of Ja
            1732, // Respect for Br'aax
            1733, // Lending Artisans a Hand
            1734, // Freezing Toes
            1735, // Giving Gatherers Their Gear
            1736, // Worqor Zormor or Bust
            1737, // Just Generally Freezing
            1738, // Comfy Cabins
            1739, // Begin with the Basics
            1740, // Cloth for Canvas
            1741, // In Line with Linen
            1742, // Weaving the Envelope
            1743, // Traditional Trousers
            1744, // Halfgloves, Full Effort
            1745, // Cruel Climates
            1746, // Glamorous Gloves
            1747, // Silk for Sunperch
            1748, // Fast-forwarding Flora
            1749, // Big Brush, Big Dreams
            1750, // Binding Spells
            1751, // For Tired Minds
            1752, // I Like Big Brush and I Cannot Lie
            1753, // Cutting Edge of Culinary Quality
            1754, // All-night Crafting
            1755, // Something Salty and Ceremonial
            1756, // Tome for Tradition
            1757, // Remedy for Reason
            1758, // More Mezcal
            1759, // Friends Are Food
            1760, // Don't Knock It Till You've Tried It
            1761, // Not-so-secret Ingredient
            1762, // Simple Is Hardest
            1763, // Creative Chocolate
            1764, // Bring Me Your Tacos
            1765, // Najoothie
            1766, // Sweet, Sweet Bean Juice
            1767, // Ocean Explosion
            1768, // Reading Reeds
            1769, // Brushing Up on Flowers
            1770, // All Stars
            1771, // Evaluating Energies
            1772, // Plants Aquatic
            1773, // First Beasts, Now Bugs
            1774, // Herbs for Hurts
            1775, // Good Ear for Corn
            1776, // Trusty Trees
            1777, // Sip of Scalebud
            1778, // Sacred Sap
            1779, // New Start for the Cinderfield
            1780, // Stopping Soapweed
            1781, // Need for Nopales
            1782, // Girth of the Earth
            1783, // Tough Tuff
            1784, // Strong Arm for Stone
            1785, // Old and Bubbly
            1786, // Abandoned Architecture
            1787, // Mountain Fish
            1788, // Gleamstone for Lazybones
            1789, // Fuelgiving Passionwork
            1790, // Building the Banks
            1791, // Pebbles for a Path
            1792, // Sampled Starfall
            1793, // Material for a Mindgenius
            1794, // Vestiges of War
            1795, // Plains, Stone, Fish
            1796, // Burning Rival
            1797, // No Takebacks for Workshirk
            1798, // Axing for Fish
            1799, // Full Moon Frogs
            1800, // Travel by Turtle
            1801, // Order for Odd Fish
            1802, // Perilous Peaks Poga
            1803, // Catch My Metaphor
            1804, // Starry-finned
            1805, // Eeling Cleanse
            1806, // Fish Tacos
            1807, // Hungry Hungry Whalaqee
        ],
    };
}
