using System.Collections.Generic;

namespace LeveHelper;

public static class Data
{
    public static readonly Dictionary<uint, uint[]> Issuers = new()
    {
        // T'mokkri
        [1000970] = [
            511, // On the Lamb
            626, // Birds of a Feather
            202, // Skillet Labor
            201, // Ain't Got No Ingots
            203, // Eyes Bigger than the Plate
            207, // You've Got Mail
            209, // Bronzed and Burnt
            208, // Headbangers' Thrall
            215, // Greavous Losses
            214, // All Ovo That
            213, // Waste Not, Want Not
            219, // Cover Girl
            221, // Kiss the Pan (Good-bye)
            220, // Fashion Weak
            225, // Night Squawker
            227, // 246 Kinds of Cheese
            226, // Get Me the Hard Stuff
            232, // A Leg to Stand On
            231, // Ingot We Trust
            233, // Insistent Sallets
            237, // Eyes on a Hard Body
            239, // Romper Stomper
            238, // Aurochs Star
            244, // Hollow Hallmarks
            243, // Very Slow Array
            245, // Get Me the Usual
            250, // Everybody Cut Footloose
            249, // Liquid Persuasion
            251, // Distill and Know that I'm Right
            255, // Feasting the Night Away
            256, // Cobalt Aforethought
            257, // Some Dragoons Have All the Luck
            143, // Mending Fences
            141, // Proly Hatchet
            142, // Hells Bells
            147, // Stainless Chef
            149, // Bring Me the Head Knife of Al'bedo Derssia
            148, // I, Gladius
            153, // Farriers of Fortune
            155, // Port of Call: Ul'dah
            154, // Anutha Spatha
            159, // Smelt and Dealt
            161, // Riveting Run
            160, // Awl or Nothing
            167, // Hearth Maul
            166, // Claw Daddy
            165, // Unseamly Conditions
            173, // Cleaving the Glim
            171, // Time to Upgrade
            172, // Mors Dagger
            177, // The Naked Blade
            178, // Out on a Limb
            179, // Can You Spare a Dolabra
            184, // Awl about Who You Know
            183, // You Spin Me Round
            185, // Spice Cadet
            190, // A Mixed Message
            189, // A Weighty Question
            191, // File That under Whatever
            196, // No Refunds, Only Exchanges
            195, // I'd Rather Be Digging
            197, // You Stay on That Side
            262, // Trout Fishing in Limsa
            263, // In Hot Water
            261, // Pork Is a Salty Food
            267, // Whip It
            269, // A Real Fungi
            268, // Jack of All Plates
            273, // Keep Your Powder Dry
            274, // Pretty Enough to Eat
            275, // Go Ahead and Dig In
            281, // A Total Nut Job
            280, // Shy Is the Oyster
            279, // Omelette's Be Friends
            285, // A Grape Idea
            287, // Feast of All Soles
            286, // Brain Food
            292, // Cooking with Gas
            291, // Convalescence Precedes Essence
            293, // Fever Pitch
            298, // Bloody Good Tart, This
            297, // Pretty as a Picture
            299, // True Grits
            303, // No More Dumpster Diving
            305, // Feeding Frenzy
            304, // Don't Turn Up Your Nose
            311, // Made by Apple in Coerthas
            310, // The Perks of Life at Sea
            309, // Moving Up in the World
            317, // Bread in the Clouds
            316, // The Egg Files
            315, // Culture Club
            701, // Always Right
            700, // Bleeding Them Dry
            699, // Nature Calls
            698, // The Palm in Your Hand
        ],
        // Gontrant
        [1000101] = [
            23, // A Clogful of Camaraderie
            22, // Touch and Heal
            21, // In with the New
            27, // Bows for the Boys
            28, // Shields for the Serpents
            29, // Spears and Sorcery
            33, // Citizens' Canes
            34, // On the Move
            35, // Raise the Roof
            41, // Driving Up the Wall
            39, // Re-crating the Scene
            40, // Nightmare on My Street
            46, // Behind the Mask
            45, // As the Worm Turns
            47, // Militia on My Mind
            52, // Tools for the Tools
            51, // Daddy's Little Girl
            53, // Armoires of the Rich and Famous
            57, // Knock on Wood
            58, // An Expected Tourney
            59, // Ceremonial Spears
            64, // A Tree Grew in Gridania
            65, // Flintstone Fight
            63, // Stay on Target
            70, // Greenstone for Greenhorns
            71, // Spin It Like You Mean It
            69, // The Arsenal of Theocracy
            76, // Clogs of War
            75, // Trident and Error
            77, // You Do the Heavy Lifting
            82, // Underneath It All
            83, // Sole Traders
            81, // Red in the Head
            88, // From the Sands to the Stage
            87, // Mind over Muzzle
            89, // In Their Shoes
            94, // The Bards' Guards
            95, // Saddle Sore
            93, // Quelling Bloody Rumors
            99, // Choke Hold
            100, // Heads Up
            101, // Skin off Their Backs
            106, // Fire and Hide
            107, // My Sole to Take
            105, // Playing the Part
            112, // Just Rewards
            111, // Men Who Scare Up Goats
            113, // Breeches Served Cold
            119, // Best Served Toad
            118, // Quality over Quantity
            117, // Emergency Patches
            124, // Soft Shoe Shuffle
            125, // Supply Side Logic
            123, // The Righteous Tools for the Job
            130, // Skirt Chaser
            129, // The Birdmen of Ishgard
            131, // The Tao of Rabbits
            135, // Hold On Tight
            137, // Handle with Care
            136, // Too Hot to Handle
            677, // Lovely Latex
            676, // We Couldn't Start the Fire
            674, // Waking Wood
            675, // East Bank Story
            680, // Evil Seeds
            679, // If a Tree Falls
            681, // No Bark, All Bite
            678, // Let the Sun Shine In
            682, // A Chest of Nuts
            685, // The Heart of the Hedge
            683, // West Bank Story
            684, // Digging Deep
            687, // Maple Stories
            688, // Never Strikes Twice
            686, // The Quick and the Dead
            689, // Briar in the Hole
            692, // Just the Artifacts, Madam
            690, // Nowhere to Slide
            691, // Shaken, Not Stirred
            693, // Fueling the Flames
            696, // Appleanche
            697, // Mushroom Gobblin'
            694, // Over the Underbrush
            695, // Moon in Rouge
        ],
        // Eustace
        [1001794] = [
            323, // Root Rush
            322, // One for the Books
            321, // Mercury Rising
            329, // A Jawbreaking Weapon of Staggering Weight
            328, // Distill, My Heart
            327, // On the Drip
            334, // Morning Glass of Ether
            333, // Wand-full Tonight
            335, // Using Your Arcane Powers for Fun and Profit
            339, // Shut Up and Take My Gil
            340, // Book and a Hard Place
            341, // Kiss of Life
            347, // The Writing Is Not on the Wall
            346, // Don't Forget to Take Your Meds
            345, // Everything Is Impossible
            353, // Sophomore Slump
            352, // Glazed and Confused
            351, // Automata for the People
            359, // Stuck in the Moment
            358, // Riches' Brew
            357, // Just Give Him a Serum
            365, // Always Have an Exit Plan
            363, // Alive and Unwell
            364, // The House Always Wins
            370, // A Bile Business
            369, // A Patch-up Place
            371, // Your Courtesy Wake-up Call
            375, // Sleepless in Silvertear
            376, // Quit Your Jib-jab
            377, // A Matter of Vital Importance
            441, // Copper and Robbers
            443, // Arms for the Poor
            442, // Needful Rings
            447, // Gods of Small Things
            448, // I Ram What I Ram
            449, // Let's Talk about Hex
            453, // All That Glitters
            455, // An Offer We Can't Refuse
            454, // The Tusk at Hand
            461, // Bad to the Bone
            459, // Brothers in Arms
            460, // Forever 21K
            467, // You Burnt It, You Bought It
            465, // Perk of Fiction
            466, // King for a Day
            472, // Thaumaturge Is Magic
            473, // All Booked Up
            471, // Love in the Time of Umbra
            478, // One Man's Trash
            479, // A Little Bird Told Me
            477, // He Has His Quartz
            484, // Lode It Up
            483, // Actually, It's Loyalty
            485, // Burning the Midnight Oil
            491, // It's My Business to Know Things
            489, // Coral on My Mind
            490, // When We Were Blings
            495, // Green and Bear It
            496, // Gold Is So Last Year
            497, // The Big Red
            382, // Trew Enough
            381, // The Unmentionables
            383, // Not Cool Enough
            388, // A Taste for Dalmaticae
            387, // The Adventurer's New Coat
            389, // Just for Kecks
            393, // Hat in Hand
            395, // Keep It under Wraps
            394, // Workplace Safety
            401, // Better Shroud than Sorry
            400, // Don't Trew So Hard
            399, // Read the Fine Print
            407, // Doublet Jeopardy
            406, // Hitting Below the Belt
            405, // New Shoes, New Me
            412, // I'll Be Your Wailer Today
            411, // Piling It On
            413, // He's Got Legs
            417, // By the Seat of the Pants
            418, // By the Short Hairs
            419, // Our Man in Ul'dah
            424, // Private Concerns
            423, // Edmelle's Hair
            425, // Crunching the Numbers
            431, // Party Animals
            430, // After the Smock-down
            429, // Cool to Be Southern
            435, // Full Moon Fever
            437, // Seeing It Through to the End
            436, // Glad As a Hatter
        ],
        // Wyrkholsk
        [1004342] = [
            510, // A Long and Winding Road
            509, // Ain't Nobody Got Time for Rats
            512, // Bug Bites
            507, // Bug Looks Like a Lady
            511, // On the Lamb
            530, // Far from the Vine
            527, // March of the Mandragoras
            526, // No Pain, No Grain
            528, // Red Stead Redemption
            529, // The Cure
            206, // Distill It Yourself
            205, // Don't Hit Me One More Time
            204, // The Alloyed Truth
            212, // Get into Their Heads
            210, // Rodents of Unusual Size
            211, // Strait Ain't the Gate
            144, // Axe Me Anything
            146, // Thank You for Your Business
            145, // The Unkindest Cut
            151, // A Hit Job
            152, // As Above, Below
            150, // Down on the Pharm
            266, // It's Always Sunny in Vylbrand
            265, // Meat-lover's Special
            264, // What a Sap
            271, // Butter Me Up
            272, // Fishy Revelations
            270, // Putting the Squeeze On
            756, // A Kelping Hand
            754, // Adventurers' Relish
            757, // Poor Boys Eat Well
            755, // Soup to Guts
            758, // Cloud Cutlet
            761, // Splendor in the Glass
            759, // They Taste Just as Pretty
            760, // Yummy in the Tummy
        ],
        // Muriaule
        [1001866] = [
            502, // No Bane, No Gain
            505, // Nutbreaker Suite
            506, // Picking Up the Piercers
            503, // Scourge of the Saplings
            504, // Stay for a Vile
            501, // Water, Water Everywhere
            522, // A Full Stomach
            520, // A Vine Finer than Twine
            524, // Compost with the Most
            521, // River Raid
            519, // We Didn't Start the Fire
            523, // Wrangling Raptors
            24, // Bowing Out
            25, // Got Your Back
            26, // Gridania's Got Talent
            31, // A Sword in Hand
            32, // Compulsory Conjury
            30, // Leaving without Leave
            85, // Sticking Their Necks Out
            86, // Tan Before the Ban
            84, // These Boots Are Made for Wailing
            91, // A Place to Call Helm
            90, // A Thorn in One's Hide
            92, // Throwing Down the Gauntlet
            675, // East Bank Story
            677, // Lovely Latex
            674, // Waking Wood
            676, // We Couldn't Start the Fire
            680, // Evil Seeds
            679, // If a Tree Falls
            678, // Let the Sun Shine In
            681, // No Bark, All Bite
        ],
        // Graceful Song
        [1003888] = [
            518, // Berries Got Bucked
            517, // Either Love Them or Kill Them
            515, // Nothing Compares to Shrews
            516, // Observe and Protect
            513, // Road Rage
            535, // A Spoonful of Poison
            533, // Needles to Say
            534, // Procession of the Noble
            532, // The Ore the Better
            536, // Two Yalms Under
            325, // Days of Chunder
            324, // Met a Sticky End
            326, // The Bleat Is On
            331, // Don't Be So Tallow
            330, // Gotta Bounce
            332, // The Hexster Runoff
            445, // Bad Bromance
            444, // Hora at Me
            446, // Water of Life
            452, // A Needle Is a Small Sword
            450, // A Ringing Success
            451, // Horn of Plenty
            385, // Burn Me Up
            384, // Hire in the Blood
            386, // Long Hair, Long Life
            391, // This Is Why You Can't Have Nice Things
            392, // Time for Acton
            390, // Wiggle Room
            717, // Miners' Holiday
            714, // Rubble Bubble
            716, // The Primrose Path
            715, // Who Needs the Paperwork
            719, // Do They Ore Don't They
            721, // I Don't Zinc It's a Coincidence
            720, // Pipikkuli's Ship Comes In
            718, // Vanishing Point
        ],
        // Swygskyf
        [1001788] = [
            547, // A Tall Odor
            551, // At the Gates
            553, // Fresh Meat
            556, // Grabbing Crabs
            549, // Jackal and the Livestock
            555, // No Egg to Stand On
            550, // Shock Therapy
            548, // The Sprite of Competition
            216, // Cook Intentions
            218, // Stadium Envy
            217, // Still the Best
            157, // I'm into Leather
            156, // Peddle to the Metal
            158, // Twice as Slice
            276, // Chew the Fat
            277, // Fisher of Men
            278, // The Bango Zango Diet
            764, // Brain Candy
            762, // Fish by Many Other Names
            763, // Just Add Water
            765, // The Fertile Incandescent
        ],
        // Tierney
        [1000105] = [
            545, // A Shroom with a View
            540, // Bump in the Night
            537, // Going Green
            542, // Hog Wild
            538, // Infestation Cessation
            544, // Mite and Madness
            543, // Scent of a Roselet
            546, // Too Close to Home
            539, // Turning Out the Blight
            541, // Wail of a Tale
            37, // Life's a Stitch
            36, // Say It with Spears
            38, // Shielding Sales
            96, // Only the Best
            97, // Simply the Best
            98, // Targe Up
            682, // A Chest of Nuts
            684, // Digging Deep
            685, // The Heart of the Hedge
            683, // West Bank Story
        ],
        // Totonowa
        [1001796] = [
            559, // Beak to Beak
            561, // Field of Beans
            565, // From Ruins to Riches
            560, // Out of Body, Out of Mind
            557, // Reeking Havoc
            558, // Skin-deep
            566, // What Peistes Crave
            563, // You Cannot Kill the Metal
            336, // One for the Road
            338, // Unbreak My Heart
            337, // You Grow, Girl
            458, // Better Four Eyes than None
            457, // Gorgeous Gorget
            456, // Point of Honor
            398, // Dirt Cheap
            396, // Making Gloves Out of Nothing at All
            397, // Welcome to the Cotton Club
            722, // Break It Up
            724, // It Peiste to Listen
            723, // That's Why They Call It Fool's Gold
            725, // We Do This the Hard Way
        ],
        // Orwen
        [1001791] = [
            575, // A Glamourous Life
            579, // Barley Legal
            577, // Beasts of Bourbon
            578, // Field Testing
            576, // First to Reave
            574, // Sucked Dry
            580, // Tail in Tow
            223, // A Firm Hand
            222, // A Well-rounded Crew
            224, // Still Crazy After All These Years
            162, // Get a Little Bit Closer
            163, // Honest Ballast
            164, // Tools of the Trade
            284, // Flakes for Friends
            283, // Rustic Repast
            282, // Sweet Smell of Success
            769, // Crab Life by the Horns
            766, // My Own Private Shell
            767, // The Deepest Cut
            768, // The Moral of the Coral
        ],
        // Qina Lyehga
        [1000821] = [
            572, // A Guest for Supper
            569, // Can't Say No to Gnat
            567, // Monkey Business
            570, // Ochus Bite, Leaves Bleed
            568, // Something in the Mead
            571, // Stew for Two
            573, // The Transporter
            44, // Bowing to Necessity
            42, // Nothing to Hide
            43, // What You Need
            104, // A Rush on Ringbands
            102, // Back in the Band
            103, // On Their Feet Again
            689, // Briar in the Hole
            687, // Maple Stories
            688, // Never Strikes Twice
            686, // The Quick and the Dead
        ],
        // Poponagu
        [1001799] = [
            583, // A Simple Solution
            584, // Death with Indignity
            586, // Earthbound
            585, // Going for Goat
            581, // The Enterprise of Undeath
            588, // The Law Won
            587, // The Missing Merchant
            344, // Devil Take the Foremost
            343, // Sanity Points
            342, // There's Something about Bury
            463, // Bad Guys Eat Brass
            464, // I Am a Rock
            462, // Sharp Words
            404, // A Drag of a Doublet
            402, // Pants Are Not Enough
            403, // Touch Me If You Can
            726, // A Man's Gotta Dream
            727, // Earth Sprites Are Easy
            728, // Eiderdown for Eidolons
            729, // The Doom that Came to Belah'dia
        ],
        // Ourawann
        [1004347] = [
            600, // Another Egg Gone
            598, // Claw-struck
            602, // Man the Ore
            601, // Strand by Me
            603, // The Deathliest Catch
            599, // The Light Stripes
        ],
        // Eugene
        [1004735] = [
            794, // Don't Touch Our Stuff
            796, // The Creeps from Work
            797, // Throw the Book at Him
            795, // Victory Is Mine, Not Yours
            808, // Double Dose of Impin'
            806, // Of Mice and Demons
            807, // Wrong and Rite
            809, // You're a Liar, Mansel Black
        ],
        // Nyell
        [1000823] = [
            589, // Belly Up
            595, // It's Hard Being Moogle
            594, // It's a Trap
            593, // Killing Beasts Softly
            590, // No Leaves Left Behind
            592, // The Root of the Problem
            596, // Treevenge
            591, // What's Yours Is Mine
            605, // More than Meets the Eye
            606, // Necrologos: Olidious Separation
            610, // Necrologos: The Beholders
            604, // Refugee Raw
            607, // Revisiting Raimdelle
            609, // Rope a 'Lope
            608, // Up the Creek
            349, // Blind Man's Bluff
            348, // Dripping with Venom
            350, // Hush Little Wailer
            354, // Conspicuous Conjuration
            356, // The Wailers' First Law of Potion
            355, // You Put Your Left Hand In
            230, // I Was a Teenage Wailer
            228, // No Hand-me-downs
            229, // Not Enough Headroom
            236, // Get Shirty
            235, // Hot for Teacher
            234, // Need for Mead
            168, // Powderpost Derby
            169, // The Devil's Workshop
            170, // When Rhalgr Met Nophica
            175, // I Saw What You Did There
            174, // Lancers' Creed
            176, // That's Some Fine Grinding
            48, // Grinding It Out
            49, // Polearms Aplenty
            50, // Wall Not Found
            56, // Heal Away
            54, // Storm of Swords
            55, // Toys of Summer
            290, // Food Fight
            288, // For Crumbs' Sake
            289, // Picnic Panic
            296, // I Love Lamprey
            295, // Love's Crumpets Lost
            294, // Whirled Peas
            469, // Dog Tags Are for Dogs
            468, // Music to Their Ears
            470, // One and Only
            476, // Dancing with the Stars
            475, // Keep the Change
            474, // Necklet of Champions
            108, // Hands On
            110, // Open to Attack
            109, // Packing a Punch
            115, // Campaign in the Membrane
            114, // No Risk, No Reward
            116, // Quicker than Sand
            408, // Getting Handsy
            409, // The Telltale Tress
            410, // Whatchoo Talking About
            416, // Bet You Anything
            414, // Pantser Corps
            415, // Put a Lid on It
            693, // Fueling the Flames
            692, // Just the Artifacts, Madam
            690, // Nowhere to Slide
            691, // Shaken, Not Stirred
            696, // Appleanche
            695, // Moon in Rouge
            697, // Mushroom Gobblin'
            694, // Over the Underbrush
            773, // Blind Ambition
            772, // Food Chain Reaction
            770, // Sounds Fishy to Me
            771, // The Long and the Shortcrust
            774, // A Shocking Soirée
            775, // A Watery Web of Lies
            776, // Fishing 101
            777, // The Truth Will Set You Free
            731, // Elemental Housekeeping
            732, // Location, Location, Location
            730, // Look How They Shine for You
            733, // Rock My Wall
            737, // Baby, Light My Way
            735, // Can't Start a Fire
            734, // Fool Me Twice
            736, // Tag, You're It
        ],
        // Cedrepierre
        [1004737] = [
            799, // A Little Constructive Exorcism
            798, // Burn It Down
            800, // Mortal Munchies
            801, // She's So Mean
            813, // A Real Wingnut
            812, // Blinded by the Wight
            810, // Go with the Flow
            811, // Sylph Strands
        ],
        // Esmond
        [1002365] = [
            611, // Dead Men Lie
            612, // Drakes' Misfortune
            614, // Flower Power
            613, // Necrologos: Fluid Corruption
            616, // Necrologos: The Noctambulist
            615, // Run, Run Away
        ],
        // Kikiri
        [1004739] = [
            802, // A Cold-blooded Business
            804, // Blood in the Water (20)
            805, // The Burning Reason
            803, // Watch Me If You Can
            817, // A Heart Aflame
            814, // Don't Tear Down This Wall
            815, // Food for Thought
            816, // The Third Prize Is that You're Slain
        ],
        // Nahctahr
        [1004344] = [
            626, // Birds of a Feather
            624, // Call Me Mating
            628, // Clearing Steer
            625, // Necrologos: Igneous Toil
            627, // Out to Sea
            623, // Sol Survivors
            629, // Under Foot
            361, // Eye of the Beholder
            362, // Growing Is Knowing
            360, // The Write Stuff
            241, // Kitty Get Your Helm
            240, // Skillet Scandal
            242, // They've Got Legs
            180, // A Spy in the House of Love
            182, // Don't Fear the Reaper
            181, // Hard Knock Life
            61, // Live Freelance or Die
            60, // The Lone Bowman
            62, // The Long Lance of the Law
            300, // Gegeruju Gets Down
            301, // Point Them with the Sticky End
            302, // Sole Survivor
            482, // Get the Green Stuff
            481, // It's Only Love
            480, // Renascence Man
            121, // Slave to Fashion
            122, // Subordinate Clause
            120, // The Hand that Bleeds
            420, // Half Is the New Double
            421, // Put on Your Party Pants
            422, // Walk Softly and Carry a Big Halberd
            701, // Always Right
            700, // Bleeding Them Dry
            699, // Nature Calls
            698, // The Palm in Your Hand
            778, // A Recipe for Disaster
            779, // Just Call Me Late for Dinner
            780, // Kitchen Nightmares No More
            781, // The Blue Period
            738, // I Kidd You Not
            741, // Shell Game
            739, // That's What the Money Is For
            740, // The Midden Fair
        ],
        // Merthelin
        [1002397] = [
            617, // A Feast in the Forest
            620, // Adamantoise Tears
            618, // Black Market Down
            619, // Necrologos: Brand of the Impure
            622, // Where the Fur's At
            621, // Woodcross Busydeal
        ],
        // Aileen
        [1002367] = [
            635, // Appetite for Abduction
            633, // Bud Bait
            630, // Coeurl Scratch Fever
            632, // Necrologos: Igneous Moil
            631, // Walk Like a Mandragora
            634, // Wonder Wine
        ],
        // Cimeaurant
        [1002384] = [
            641, // Cower to the People
            637, // Have a Nice Trip
            640, // Little Lost Lamb
            639, // Meat and Bleat
            638, // Necrologos: Cinerary Rite
            636, // Now We're Even
            642, // Saving Bessy
            368, // Going Nowhere Fast
            366, // Open Your Grimoire to Page 42
            367, // The Sting of Conscience
            248, // I've Got You under My Skin
            247, // Skillet to the Stars
            246, // War Is Tough on the Hands
            187, // Colder than Steel
            186, // Lending a Hand
            188, // Seemed Like the Thing to Get
            68, // Bend It Like Durendaire
            66, // Grippy When Wet
            67, // The Cold, Cold Ground
            308, // Leek Soup for the Soul
            307, // Rise and Dine
            306, // Winter of Our Discontent
            487, // Dead Can't Defang
            488, // Faith and Fashion
            486, // Wear Your Patriotic Pin
            128, // First They Came for the Heretics
            126, // Springtime for Coerthas
            127, // Through a Glass Brightly
            428, // A Leg Up on the Cold
            427, // In over Your Head
            426, // The Wages of Sin
            702, // A Stash of Herbs
            705, // Catch My Drift
            704, // Salad Days
            703, // Spear of Heaven
            784, // Fry Me a River
            785, // Gathering Light
            783, // Hands off Our Fish
            782, // The Perks of Politics
            744, // Eye for an Eye
            745, // Mythril Madness
            742, // Rocks for Brains
            743, // There Are No Good Answers
        ],
        // Haisie
        [1007068] = [
            844, // Dress for Aggress
            833, // Fanning the Flames
            838, // Feathered Foes
            834, // Fishing off the Company Pier
            839, // Just Making an Observation
            835, // Mad about You
            845, // Pick Your Poison
            840, // Roast Lamb with Mint and Hellfire
            843, // The Baddest Brigade in Town
        ],
        // C'lafumyn
        [1004736] = [
            819, // It's Better (for You) Under the Sea
            818, // Road Worriers
            821, // Twenty-nine Was the Cutoff
            820, // You Are NOT a Pirate
        ],
        // H'amneko
        [1004738] = [
            824, // Creature Feature
            826, // Dead Man Walking
            825, // It Goes with the Territory
            823, // This Is Going to Sting, A Lot
        ],
        // Blue Herring
        [1004740] = [
            829, // And Then There Were None
            828, // Bridges of Qiqirn Country
            831, // Grapevine of Wrath
            830, // The Cost of Living
        ],
        // Rurubana
        [1002398] = [
            648, // Blow-up Incubator
            646, // Circling the Ceruleum
            645, // Don't Forget to Cry
            644, // Necrologos: Pale Oblation
            647, // Someone's in the Doghouse
            643, // Subduing the Subprime
        ],
        // Voilinaut
        [1002401] = [
            650, // Got a Gut Feeling about This
            649, // Necrologos: Whispers of the Gem
            654, // Pets Are Family Too
            652, // The Area's a Bit Sketchy
            653, // The Tropes of the Trade
            655, // We Can Dismember It for You Wholesale
            651, // You Look Good Enough to Eat
            374, // A Real Smooth Move
            373, // Arcane Arts for Dummies
            372, // No Accounting for Waste
            254, // Employee Retention
            253, // Family Secrets
            252, // Metal Fatigue
            193, // Get Me to the War on Time
            192, // Kitchen Casualties
            194, // Streamlining Operations
            72, // A Winning Combo
            74, // Ready for a Rematch
            73, // The Turning Point
            313, // Good Eats in Ishgard
            314, // Pagan Pastries
            312, // Rolanberry Fields Forever
            494, // If You've Got It, Flaunt It
            492, // North Ore South
            493, // Tough Job Market
            132, // Foot Blues
            134, // It's Not a Job, It's a Calling
            133, // Not So Alike in Dignity
            434, // A Matter of Import
            432, // I'll Swap You
            433, // No Country for Cold Men
            706, // Plague on Both Our Forests
            709, // Sign of the Crimes
            707, // Straight and Arrow
            708, // This Old Fort
            786, // Empire Builder
            787, // Laird of the Lakes
            788, // Make a Fish
            789, // Rationally Speaking
            748, // Breach and Build
            747, // Brother in Arms
            746, // Nature Is a Monster
            749, // Not Losing Our Heads This Time
        ],
        // Lodille
        [1007069] = [
            849, // An Imp Mobile
            860, // If You Put It That Way
            859, // No Big Whoop
            850, // Papal Dispensation
            848, // Someone's Got a Big Mouth
            854, // Talk to My Voidsent
            855, // The Bloodhounds of Coerthas
            853, // Yellow Is the New Black
            858, // You Dropped Something
        ],
        // K'leytai
        [1004348] = [
            661, // A Toad Less Taken
            658, // Big, Bad Idea
            657, // Necrologos: The Liminal Ones
            660, // One of Our Naturalists Is Missing
            659, // Put Your Stomp on It
            656, // Turnabout's Fair Play
            379, // Make Up Your Mind or Else
            380, // Not Taking No for an Answer
            378, // Shut Up Already
            259, // Booty Call
            260, // Dealing with the Tough Stuff
            258, // Parasitic Win
            200, // I Maul Right
            198, // Pop That Top
            199, // Talon Terrors
            78, // Bow Down to Magic
            79, // Bowing to Greater Power
            80, // Incant Now, Think Later
            318, // Comfort Me with Mushrooms
            319, // Drinking to Your Health
            320, // Red Letter Day
            500, // Light in the Darkness
            498, // Sew Not Doing This
            499, // Sweet Charity
            138, // Fuss in Boots
            140, // Spelling Me Softly
            139, // Tenderfoot Moments
            439, // And a Haircut Wouldn't Hurt
            438, // Big in Mor Dhona
            440, // Bundle Up, It's Odd out There
            710, // Caught in the Long Grass
            713, // See How They Shine
            711, // Topsy-turvy Time
            712, // Wonders Don't Cease, They're Discontinued
            791, // Awash in Evidence
            790, // Putting the Zap on Nature
            793, // Sleeper Creeper
            792, // Snail Fail
            751, // Crystal Mess
            753, // Hybrid Hypotheses
            750, // Metal Has No Master
            752, // Sucker Seer
        ],
        // Eidhart
        [1007070] = [
            869, // Amateur Hour
            870, // Get off Our Lake
            865, // Go Home to Mama
            864, // Kill the Messenger
            863, // One Big Problem Solved
            874, // Science Shindig
            868, // The Awry Salvages
            875, // The Museum Is Closed
            873, // Who Writes History
        ],
        // Eloin
        [1011208] = [
            883, // Necrologos: Of Sallow Vizards (L)
            881, // We're So Above This (L)
            879, // Don't Come Back
            880, // Necrologos: Of Sallow Vizards
            878, // The Unexpected Tourist
            882, // The Second Coming of Yak (L)
            884, // Quit Loafing Around
            887, // Last Priest Profaned (L)
            886, // Jailbird Break
            889, // Return to Sender (L)
            885, // Feathers Unsullied
            888, // Scavenger Hunt (L)
            891, // Puppet Show
            895, // Hello, Cousin (L)
            894, // More Than One Way (L)
            893, // Family Comes First (L)
            892, // Cry Home
            890, // Show Your Work
            898, // Marl-ementary Procedure
            901, // Nobody Can Farm Marl, but Marl (L)
            900, // Needs More Fervor (L)
            899, // Just Washed (L)
            896, // Two Birds, One Culling
            897, // Don't Eat the Shrooms
            907, // Bareback Riding (L)
            906, // Necrologos: His Treasure Forhelen (L)
            903, // Necrologos: His Treasure Forhelen
            905, // Goblin Up Sharlayan (L)
            902, // Dance, Magic Dance
            904, // Whither the Morbol Goeth
            1063, // Dodging the Draft (L)
            1059, // Summoning for Dummies
            1062, // Summoning the Courage to Be Different (L)
            1061, // Forgery of Convenience (L)
            1060, // Forged from the Void
            1058, // The Mustache Suits Him
            1068, // Surgical Substitution (L)
            1065, // Steeling the Knife, Steeling the Mind
            1069, // Curbing the Contagion (L)
            1066, // Consecrating Congregation
            1064, // Can't Sleep, Inquisitors Will Eat Me
            1067, // Allow No Fallacies (L)
            1071, // Tomes Roam on the Range
            1074, // Field Trip to the Unknown (L)
            1072, // Warding Off Temptation
            1075, // The Garden of Arcane Delights (L)
            1070, // Adhesive of Antipathy
            1073, // It's Gonna Grow Back (L)
            1077, // Volunteering with Staff
            1079, // Washing Away the Sins (L)
            1080, // Scripture Is the Best Medicine (L)
            1078, // Rolling on Initiative
            1081, // Darkly Dreaming Dexterity (L)
            1076, // Cleansing the Wicked Humours
            1085, // Ink into Antiquity (L)
            1084, // The Grave of Hemlock Groves
            1082, // Filling in the Blanks
            1083, // There Was a Late Fee
            1087, // Dappling the Highlands (L)
            1086, // A Gate Arcane Is Dragon's Bane (L)
            999, // Rivets Run through It
            1002, // A Riveting Revival (L)
            1000, // Don't Scuttle with Scuta
            998, // Hauberk and No Play
            1003, // Shielded by Bureaucracy (L)
            1001, // Knights without Armor (L)
            1008, // Sheer Distill Power (L)
            1009, // Skillet with Fire (L)
            1005, // The Cut Alembical Cord
            1007, // Fifty Shields of Blades (L)
            1004, // Let Faith Light the Way
            1006, // Pan That Laid the Golden Egg
            1014, // Rage against the Scream (L)
            1013, // Heavy Metal Banned (L)
            1011, // Someone Put Dung in My Helmet
            1012, // Sometimes the South Wins
            1010, // As the Bolt Flies
            1015, // The Thriller of Autumn (L)
            1017, // A Halonic Masquerade
            1020, // Why I Wear a Mask (L)
            1021, // Shouldering the Shut-ins (L)
            1018, // Belle of the Brawl
            1019, // All's Fair in Highborn Assassination (L)
            1016, // A Squire to Inspire
            1022, // Sir, Dost Thou Even Heft
            1025, // Men in Adamantite (L)
            1023, // Look Before You Leap
            1024, // The Mast Chance
            1027, // The Rose and the Riveter (L)
            1026, // Patience, Young Grasshopper (L)
            970, // With Bearings Straight
            972, // Foreign Exchange (L)
            971, // Starting Young (L)
            968, // Barring the Gates to Foundation
            969, // Punching Your Way to Success
            973, // Bearing the Brunt (L)
            974, // It's All about Execution
            977, // Too Big to Miss (L)
            976, // Cautionary Cutlery
            979, // Saw, Shank, and Redemption (L)
            975, // Freight and Barrel
            978, // Stepping on My Heart with Stilettos (L)
            983, // Tensions in Creasing (L)
            985, // Unconventional Weaponry (L)
            984, // I Came, I Sawed, I Conquered (L)
            980, // I Could Feel That from Here
            981, // I Saw the Pine
            982, // Keep Up with the Mechanics
            987, // Diamond Sawdust
            986, // Unbreaker
            991, // The Clamor for Hammers (L)
            988, // Spirituality Inspector
            990, // I'm a Lumberjack and I'm Okay (L)
            989, // Attack on Titanium (L)
            994, // Swords for Plowshares
            996, // The Nightsoil Is Dark and Full of Terrors (L)
            992, // Through Thick and Thin
            995, // Piercing Eyes Deserve Piercing Shafts (L)
            997, // Negative, They Are Meat Popsicles (L)
            993, // Winter Weather Conditions
            913, // Living Bow to Mouth (L)
            910, // Almost as Fun as Slingshotting Birds
            911, // The Lumber of Their Discontent (L)
            912, // Sticks and Stones (L)
            908, // Splinter in the Sewers
            909, // So You Think You Can Lance?
            916, // A Reward Fitting of the Faithful
            917, // Win One Bow, Get Three Free (L)
            914, // Do You Even String Bow
            918, // Fishing for Profits (L)
            919, // Just Rewards for Just Devotion (L)
            915, // Landing the Big One
            922, // Walking on Pins and Needles
            921, // The Darkest Hearth
            923, // Purified Polyrhythm (L)
            925, // Like Lemon on a Lumbercut (L)
            920, // License to Heal
            924, // Fruit of the Loom (L)
            930, // A Sky Pirate's Life for Me (L)
            927, // Don't Ask Wyvern
            926, // The Long Armillae of the Law
            931, // To Protect My City, I Must Wear a Mask (L)
            928, // Aim to Please
            929, // Wooden Ambitions (L)
            936, // Pulling Them to the Grind (L)
            934, // Hold on Adamantite
            937, // Spears for Stone Vigilantes (L)
            935, // Built This City on Blocks and Soul (L)
            932, // Birch, Please
            933, // Anatomy of a Drill Bit
            1031, // Confections of Confession (L)
            1029, // The Next to Last Supper
            1030, // The Aroma of Faith
            1028, // Little Orphan Candy
            1033, // Soup's On (L)
            1032, // Nostalgia through the Stomach (L)
            1036, // Persona non Gratin
            1034, // Such a Butter Face
            1035, // Loving That Muffin Top
            1039, // Recipe for Disaster (L)
            1038, // Muffin of the Morn (L)
            1037, // No Margarine of Error (L)
            1043, // Time for a Midnight Snack (L)
            1040, // The Nutcracker's Sweets
            1045, // The Eats of Authenticity (L)
            1042, // Old Victories, New Tastes
            1041, // Breakfast of Champions
            1044, // Emerald Soup for the Soul (L)
            1048, // Persuasion of a Higher Power
            1050, // Quenching the Flame (L)
            1046, // Saucy for a Suitor
            1049, // Saved by the Sauce (L)
            1051, // Loaves and Fishes (L)
            1047, // It Goes Down Smoothly
            1055, // Luxury Spillover (L)
            1053, // Soup That Eats Like a Knight
            1056, // Like Ma Used to Make (L)
            1052, // Let's Not Get Sappy
            1054, // Don't Let It Fall Apart
            1057, // Better Come Back with a Sandwich (L)
            1119, // Not on My Table
            1121, // Peril Never Wore Safety Goggles (L)
            1118, // The Goggles, They Do Naught
            1123, // Transposing Theology (L)
            1120, // Halonic Hermeneutics
            1122, // Heinz's Dilemma (L)
            1124, // Sense of Entitlement
            1128, // Old-school Spooling (L)
            1129, // Hulls of Broken Dreams (L)
            1127, // Charting the Trends (L)
            1125, // High Above Me, She Sews Lovely
            1126, // Sky Is the Limit
            1134, // Watchers within the Walls (L)
            1130, // The Unfortunate Retirony
            1133, // Life Ends at Retirement (L)
            1132, // The Monuments Mages
            1135, // Deal with It (L)
            1131, // Citizen's Arrest
            1137, // The Grander Temple
            1139, // With a Noise That Reaches Heaven (L)
            1141, // Silver Bar of Upcycling (L)
            1138, // Appeasing the Astromancer
            1136, // Needs More Prayerbell
            1140, // Man with a Dragon Earring (L)
            1145, // Ring of Reciprocity (L)
            1146, // The Lovely Hands of Haillenarte (L)
            1142, // Keeping Claw and Order
            1147, // It's the Circlet of Life (L)
            1143, // Embroiling Embroidery
            1144, // A Halo for Her Head
            943, // Treat Them with Kid Gloves (L)
            942, // These Boots Are Made for Hawkin' (L)
            939, // From Mud to Mourning
            940, // Glorified Hole-punchers
            941, // The Style of the Time (L)
            938, // Pummeling Abroad
            946, // (Don't) Love the Skin You're In
            945, // Maybe He's a Lion
            949, // Wrist Apart (L)
            947, // They Call It Bloody Mary (L)
            948, // The Wyvern of It (L)
            944, // You Could Say It's a Moving Target
            955, // Exploiting the Adroit (L)
            950, // Overall, We Blend In
            954, // Hunting Heretics (L)
            952, // Dragoon Drop Rate
            951, // Tally Ho, Chocobo
            953, // Eviction Notice (L)
            959, // It's All in the Wrists (L)
            956, // Don't Sweat the Small Fry
            961, // Training Is Only Skintight (L)
            958, // Trainin' the Neck
            960, // Halonic Drake Handlers (L)
            957, // I Need Your Glove Tonight
            964, // It Will Knock Your Socks Off
            962, // Starting Off on the Wrong Foot
            965, // Raising the Dragoons (L)
            963, // Bar of the Bannermen
            966, // Do My Little Turn on the Stonewalk (L)
            967, // On My Own Two Feet (L)
            1089, // Protecting the Foundation
            1088, // Pride Up in Smoke
            1090, // Ribbon of Remembrance
            1093, // The Road Was a Ribbon of Moonlight (L)
            1092, // Curb the Gnawing Feeling (L)
            1091, // Desperate for Diversionaries (L)
            1099, // Soot in My Hair and Scars on My Feet (L)
            1096, // An Account of My Boots
            1094, // What Not to Wear
            1095, // Fashion Patrol
            1098, // Dress Code Violation (L)
            1097, // Appeal of Foreign Apparel (L)
            1102, // Finger on the Pulse
            1100, // Clothing the Naked Truth
            1101, // Storm upon Bald Mountain
            1105, // Chirurgeon Hand in Glove (L)
            1103, // When in Robes (L)
            1104, // Abrupt Apprentices (L)
            1110, // To Kill a Dragon on Nameday (L)
            1107, // Where the Dragonflies, the Net Catches
            1109, // Pants Fit for Battle (L)
            1111, // Maids of Honor (L)
            1108, // Investing in the Future
            1106, // Healing with Flair
            1116, // Pom Hemlock (L)
            1114, // The Hat List
            1113, // He Wears the Pants
            1112, // Felt for the Fallen
            1115, // Blinded Veil of Vigilance (L)
            1117, // Knight Incognito (L)
            1152, // A Crown for Every Head (L)
            1150, // Warm in Their Beds
            1149, // Breathe Deeply
            1151, // Secondhand Smoke Screen (L)
            1148, // Does This Look Infected?
            1153, // No Rest for the Thicket (L)
            1154, // Ladies and Gentians
            1158, // The Sour Patch Grids (L)
            1155, // The Bitter, the Better
            1156, // Six Hours in a Leaky Boat
            1157, // Coat the Harm (L)
            1159, // Hot Tub Clime Machine (L)
            1164, // Sacrilege Neutralized (L)
            1160, // Your Mother Is Medicine and Your Father Is the Wild
            1165, // Hybrid Theories (L)
            1163, // Exotic Remedies (L)
            1161, // Dandelion Don't Tell No Lies
            1162, // Chewed Up and Snuffed Out
            1171, // Nectar of the Goddess (L)
            1166, // Heart to Heart
            1169, // Putting on Airs (L)
            1168, // The Anointing of the Dead
            1167, // Bold and Blue
            1170, // These Colors Run Not (L)
            1174, // A Taste of Their Own Medicine
            1177, // Thank You for Smoke Screening (L)
            1175, // Blending In (L)
            1173, // Bleeding Out
            1172, // Watching the Western Wall
            1176, // Mending Wings (L)
            1212, // A Whole Lot of Nope (L)
            1210, // Dine or Spine
            1209, // The Voice of the Fury
            1211, // Snipped for Spirituality (L)
            1213, // Lurchin' from Urchins (L)
            1208, // Please Halone, Tell Me I'm Still Asleep
            1216, // Bounty of Sky, Bounty of Earth
            1217, // Prayer and Prejudice (L)
            1218, // Valuing the Vintage (L)
            1219, // Eating Like the Natives (L)
            1215, // Fish Oils and Forgotten Spoils
            1214, // Loose Lips Heal (Broken) Hips
            1220, // Analysis of Paralysis
            1221, // The Aquariums of Ishgard
            1225, // Blue of Sky and Sea (L)
            1223, // Send a Feeling to My Spine (L)
            1224, // A Win-win Situation (L)
            1222, // Bearing of the Blue
            1231, // Plus One or Two or Three (L)
            1228, // Pipira Pirouette
            1227, // Solo Out the Bolo
            1230, // Sucking on Face (L)
            1229, // Hundred Fins for a Hundred Wings (L)
            1226, // Dining with Dravanians
            1236, // Angling for Ailments (L)
            1232, // Spew Forth and Spawn
            1233, // They Call It the Kissing Disease
            1235, // Unleash the Hydro Cannons (L)
            1234, // Warmer than Wine
            1237, // What Does Not Break Us, Devours Us (L)
            1181, // The Basics of Forgery (L)
            1183, // Breaking Beacons (L)
            1179, // The Road to Pilgrimage
            1180, // The Merits of Upcycling
            1182, // For Vares Beyond Compare (L)
            1178, // Taken for Granite
            1187, // Rose Never Lets Go (L)
            1189, // Polished till They Shine (L)
            1184, // Permit for Destruction of Religious Property
            1188, // Forging Lance Base (L)
            1186, // Halone's Jewelry Box
            1185, // I'll Show You My Battle Shards
            1192, // Talk about Boundaries
            1193, // All of These Bases Belong to Us (L)
            1190, // From Creepers to Squatters
            1195, // History Needs Some Revisions (L)
            1191, // Dreams of War, Dreams of Liars, Dreams of Dragon Fire
            1194, // There's Sand in My Water (L)
            1200, // The Puppets of War (L)
            1201, // A Spire for New Heights (L)
            1197, // What Goes Up
            1198, // Mortarin'
            1199, // Fool Me Once (L)
            1196, // Fake-icite
            1202, // Sticking It Out
            1207, // Sharlayan Sympathizers (L)
            1205, // Pommeling the Enemy (L)
            1204, // Dragonproofing
            1203, // Crystal Chronicles
            1206, // Not So Crystal Clear (L)
        ],
        // Keltraeng
        [1018997] = [
            1315, // Magic Beans
            1314, // Spellbound
            1313, // Whinier than the Sword
            1318, // Materia Worth
            1316, // Official Strategy Guide
            1317, // Scroll Down
            1321, // Asking for a Friend
            1320, // Rumor Has It
            1319, // The Dotted Line
            1322, // Edge of the Arcane
            1324, // Let Loose the Juice
            1323, // Spell-rebound
            1327, // Making Your Mark
            1325, // Pep-stepper
            1326, // Ultimate Official Strategy Guide
            1283, // Mail It In
            1284, // Alembic Medals
            1285, // Setting the Stage
            1288, // Ore for Me
            1286, // Shielded Life
            1287, // The Gauntlet Is Cast
            1291, // En Garde and on Guard
            1289, // Greaving
            1290, // Home Cooking
            1292, // Art Imitates Life
            1293, // Smells of Rich Tama-hagane
            1294, // Sweeping the Legs
            1297, // Heads Will Roll
            1295, // See Shields by the Sea Shore
            1296, // Spoony Is the Bard
            1269, // A Knack for Nicking
            1268, // Have Blade, Will Travel
            1270, // High Steal
            1271, // Crisscrossing
            1273, // File under Dull
            1272, // Hammer Time
            1276, // And My Axe
            1274, // Killer Cutlery
            1275, // Meddle in Metal
            1278, // Renting Mortality
            1277, // Sea-saw
            1279, // The Bigger the Blade
            1282, // Fire for Hire
            1280, // Hammer and Sails
            1281, // Ingot to Wing It
            1239, // Reeling for Rods
            1240, // Beech, Please
            1238, // Walk the Walk
            1242, // Composition
            1241, // Standing on Ceremony
            1243, // Wood That You Could
            1245, // O Pine
            1244, // Pinewheel
            1246, // Run Before They Walk
            1247, // Everybody's Heard about the 'Berd
            1249, // Spare a Rod and Spoil the Fishers
            1248, // The Ear Is the Way to the Heart
            1250, // Putting Your Line on the Neck
            1252, // With a Bow on Top
            1251, // Zelkova, My Love
            1299, // Loquacious
            1300, // All You Can Stomach
            1298, // Oh No Udon
            1302, // Hunger Is No Game
            1301, // Soup for the Soul
            1303, // The Frier Never Lies
            1305, // A Shorlonging for the Familiar
            1306, // Souper
            1304, // Sweet Kiss of Death
            1307, // No Othard Choice
            1308, // Persimmony Snicket
            1309, // West Meats East
            1311, // Fish Box
            1310, // Fits to a Tea
            1312, // Herky Jerky
            1344, // One Ring Circus
            1343, // Play It by Ear
            1345, // Wants and Needles
            1347, // Bracelet for Impact
            1346, // Chain of Command
            1348, // If I'd a Koppranickel for Every Time...
            1349, // Cutting Deals
            1351, // Hair-raising Action
            1350, // Needle in a Hingan Stack
            1352, // Best-laid Planispheres
            1353, // Put the Metal to the Peddle
            1354, // Ring in the New
            1357, // Choker in the Clutch
            1355, // Speak Softly and Carry a Metal Rod
            1356, // Untucked
            1255, // Fitting In
            1254, // Hide to Go Seek
            1253, // Vested Interest
            1258, // Looking for Glove
            1257, // Off the Cuff
            1256, // Weathering Heights
            1260, // Shoe on the Other Foot
            1261, // Tiger in the Sack
            1259, // Try Tricorne Again
            1262, // A Stitch in Time
            1264, // Security Breeches
            1263, // Shrug It On
            1265, // Brace Yourselves
            1267, // If the Shoe Fits
            1266, // Thick and Thin
            1328, // Modest Beginnings
            1330, // Proper Props
            1329, // What Guides Want
            1331, // Duress Rehearsal
            1333, // Getting a Leg Up
            1332, // Skills on Display
            1334, // Apparent Apparel
            1336, // Of Great Import
            1335, // Say Yes to Formal Dress
            1338, // Don't Sweat the Role
            1339, // To the Tops
            1337, // Who War It Better
            1340, // Brimming with Confidence
            1342, // Cap It Off
            1341, // One Winged Angle
            1358, // Rhalgr Wood Too
            1360, // Barking Up the Right Tree
            1359, // Thank Heavenspillar
            1362, // Coral-lation
            1363, // Penned-up Frustration
            1361, // Pining
            1364, // Bamboozled
            1366, // Flowers for Algae Run
            1365, // Timbr
            1367, // Craic of Dawn
            1368, // Hypocritic Oath
            1369, // Leaves Much to Be Desired
            1372, // Garden Variety
            1371, // Last of the Mhigans
            1370, // Nunh the Wiser
            1389, // If a Leaf Falls in the Water
            1388, // Slow Wash, Rapids Jumper
            1390, // There Can Be Only One
            1392, // In a Pickle
            1391, // Lighter Wallets
            1393, // Perhaps Not-So-Common
            1394, // Catfish Scheme
            1395, // Curtains for Pleco
            1396, // Marooned Minnow
            1397, // Peculiar De-light
            1398, // Step by Steppe
            1399, // Unbeliebubble
            1402, // Pre-octopied
            1401, // To the Teeth
            1375, // Axe to Grind
            1373, // Cermet Breaker
            1374, // Set in Stone
            1378, // Coral-lary
            1377, // Hit Rock Bottom
            1376, // Ready Ore Not
            1381, // No Stone Unturned
            1380, // Scraptacular
            1379, // Simply Marble Us
            1382, // Bead 'Em Up
            1384, // Dunes of Our Lives
            1383, // O Say Can You Rock
            1386, // Adios, Ala Mhigo
            1385, // Mine All Mine
            1387, // The Ores Have It
            1400, // Blood in the Water
        ],
        // Eirikur
        [1027847] = [
            1478, // Another Man's Ink
            1480, // A Time for Peace
            1479, // Keeping Magic Alive
            1482, // An Eye for Healing
            1481, // Make It Bigger
            1483, // Making Ends Meet
            1486, // 5-bell Energy
            1484, // Amaro Kart
            1485, // Conserving Combat
            1488, // A Greater Grimoire
            1489, // Crafty Concoctions
            1487, // Growing Up
            1490, // A Labor of Love
            1492, // Mindful Medicine
            1491, // Supreme Official Strategy Guide
            1450, // A Head of Demand
            1448, // Shielding the Realm
            1449, // Time to Fry
            1452, // Hedging Bets
            1451, // Scheduled Maintenance
            1453, // Wrapped Knuckles
            1455, // A New Regular
            1454, // Catching an Earful
            1456, // The Proper Precautions
            1459, // A Budding Business
            1457, // No Scope
            1458, // Signed, Shield, Delivered
            1461, // One Foot Forward
            1462, // Shield to Shield
            1460, // Trial and Error
            1434, // Fire Sale
            1435, // Here Comes the Hammer
            1433, // The Gold Experience
            1438, // Enlistment Highs
            1437, // Heavy Hitter
            1436, // Selective Logging
            1440, // Halfhearted Effort
            1439, // Instruments of Distraction
            1441, // Nip It in the Bud
            1444, // Cooking for the Future
            1442, // Dodge Once, Cut Twice
            1443, // Idol Hands
            1445, // Bae Blade
            1447, // Keeping Loyalty
            1446, // Under the Fool Moon
            1403, // Built to Last
            1404, // Just Starting Out
            1405, // Playing the Market
            1406, // A Stronger Offense
            1407, // Taking Aim
            1408, // Understaffed
            1410, // Ground to a Halt
            1411, // Horde of the Rings
            1409, // Patient Patients
            1414, // A Miss and a Hit
            1413, // Sleep on It
            1412, // The Right Tool for the Job
            1416, // Kindling the Flame
            1415, // Off to a Good Staff
            1417, // Safety First
            1464, // Meet for Meat
            1465, // Cure for What Ails
            1463, // Slippery Service
            1466, // His Dark Utensils
            1467, // Soup for the Soldier
            1468, // Sweet Tooth
            1469, // Can't Eat Just One
            1471, // Mixology
            1470, // One Last Meal
            1473, // A Good Omen
            1472, // On a Full Stomach
            1474, // Teetotally
            1477, // A Cookie for Your Troubles
            1476, // A Happy End
            1475, // Super Dark Times
            1508, // Whetstones for the Workers
            1509, // Satisfactory Sewing
            1510, // You're My Wonderhall
            1513, // Gentleman Donor
            1512, // Neck on the Line
            1511, // Slimming Down
            1514, // Copious Crystal Cannons
            1515, // Hot Rod
            1516, // Unsung Generosity
            1519, // A Magnanimous Refrain
            1518, // Birth Ring
            1517, // Prophet of Profit
            1521, // A Beneficent Elegy
            1520, // Bulking Up
            1522, // Wrap Those Wrists
            1420, // Band Substances
            1419, // Girding for Glory
            1418, // Ware and Chair
            1423, // A Slippery Slope
            1421, // Breeches of Trust
            1422, // Glove Me Tender
            1425, // A Heady Endeavor
            1426, // At Your Neck and Call
            1424, // Peace in Rest
            1428, // If I Could Walk a Thousand Malms
            1427, // Protecting the Nuts
            1429, // Strike True
            1432, // A Shoe In
            1430, // Fit for a Friend
            1431, // Into the Storm
            1494, // Lovely Leggings
            1493, // Flax Wax
            1495, // Turban in Training
            1498, // Hair Do No Harm
            1497, // Legs for Days
            1496, // Suits You
            1499, // A Tender Table
            1501, // Gloves Come in Handy
            1500, // Hunting Season
            1502, // All-purpose Overgarments
            1504, // Something in My Eye
            1503, // The Hunt Continues
            1505, // A Job Well Done
            1506, // A Turban for the Ages
            1507, // Healing Headwear
            1525, // Home Is Where the Heart Is
            1524, // Medicinal Herbs
            1523, // Sought-after Spices
            1527, // Home Is Where the Heart Isn't
            1526, // Packs a Punch
            1528, // Seeds for the Sick
            1531, // Dream a Little Dream
            1530, // Fresh off the Boat
            1529, // Pest Problems
            1533, // Culinary Concepts
            1532, // Spiritual Ventures
            1534, // The Only Cure
            1537, // Big Business
            1535, // Good Business
            1536, // The Sweetest Syrup
            1553, // Eco-Warrior of Light
            1554, // Needs More Egg
            1555, // The Source of the Problem
            1558, // Aetherquake
            1557, // Full of Fish
            1556, // Jelly Salad
            1561, // A Cherry-red Herring
            1559, // Faerie Fish
            1560, // The Bride Aquatic
            1562, // Crab Corps
            1564, // Fish for Days
            1563, // Magic Mushrooms
            1567, // A Feast for the Senses
            1566, // Deep-sea Diving
            1565, // Short-term Gains
            1540, // Barmy for Ballistas
            1538, // Jewelry for All
            1539, // The Search for Slag
            1543, // Crystallized Revenge
            1542, // New Necklaces
            1541, // Secret Stones
            1545, // Jewels for Jewelry
            1546, // Knowledge Is Power
            1544, // Rocks from Rak'tika
            1548, // Jewelry Is Forever
            1547, // Road to Recovery
            1549, // The Magic of Mining
            1551, // Back Stronger
            1552, // Crystal Meds
            1550, // Lakeland's Legacy
        ],
        // Grigge
        [1037263] = [
            1629, // Nearly Bare
            1628, // Wishful Inking
            1631, // Body over Mind
            1630, // Luncheon Bound
            1633, // Liquid Competence
            1632, // Rebuilding to Code
            1634, // Nearly There
            1635, // Practical Command
            1637, // Mindful Study
            1636, // Technically Still Magic
            1589, // The Armoire Is Open
            1588, // Haste for High Durium
            1590, // Ace of Gloves
            1591, // The Incomplete Costume
            1592, // Armoire Aftercare
            1593, // Once and for Alchemy
            1594, // Heading toward Bankruptcy
            1595, // In-kweh-dible Cooking
            1596, // A Gift of Gloves
            1597, // Additions to the Armoire
            1579, // Archon Denied
            1578, // To Delight a Dancer
            1581, // Archon of His Eye
            1580, // History of the Hrothgar
            1583, // In Pursuit of Panaloaf
            1582, // Records of the Republic
            1584, // Mangalomania
            1585, // Pruned to Perfection
            1586, // Annals of the Empire I
            1587, // Plying with Precision
            1569, // A Real Grind
            1568, // Timber of Tenkonto
            1571, // A Wristy Experiment
            1570, // Earring Awakening
            1572, // A Better Conductor
            1573, // In Rod We Trust
            1574, // An A-prop-riate Request
            1575, // Spinning the Time Away
            1577, // An Integral Reward
            1576, // Annals of the Empire II
            1638, // Salt of the North
            1639, // Topping Up the Pot
            1641, // At Any Temperature
            1640, // Bobbing for Compliments
            1643, // A Stickler for Carrots
            1642, // Imperial Palate
            1644, // An Historical Flavor
            1645, // Comfort Food
            1646, // Blast from the Pasta
            1647, // The Mountain Steeped
            1598, // Awarding Academic Excellence
            1599, // Workplace Workout
            1601, // Pewter-hewn Punishment
            1600, // The Sage's Successor
            1602, // Gold Rush Order
            1603, // Sage with the Golden Earrings
            1605, // The Needle That Binds
            1604, // To Fight at Her Side
            1606, // Planisphere to Paper
            1607, // Star Athletes
            1609, // Running up the Tabi
            1608, // Hell on Leather
            1610, // Boot Legs
            1611, // Scouting Talent
            1612, // Battered Books
            1613, // Loyal Turncoat
            1614, // Grips of Fear
            1615, // Loving Soles
            1617, // For What Was Gleaned
            1616, // Generous Soles
            1618, // Heavy Armoire
            1619, // Helping Handwear
            1621, // Color Coated
            1620, // Hot Heads
            1622, // A Polished Purchase
            1623, // Turban Sprawl
            1625, // Lifetime of Gleaning
            1624, // Lightening Up
            1627, // A Better Bottom Line
            1626, // Skill Cap
            1649, // A Balanced Diet
            1650, // Bug Report
            1648, // Paper Minds
            1651, // Don't Have a Yakow
            1653, // Explosive Palms
            1652, // Soup for the Stars
            1654, // Beet It
            1655, // Into the Pines
            1656, // Through the Fires and Flames
            1659, // In Case of Emergency
            1657, // Tea Off
            1658, // When Size Matters
            1660, // A Natural Style
            1662, // Poisonous Palms
            1661, // Wood Envy
            1679, // Crabs for the Crabby
            1678, // Fish for Thought
            1681, // Fishing for the Future
            1680, // Water Works
            1682, // Making Waves
            1683, // What Would You Do for a Pickle
            1684, // Fungi of the Firmament
            1685, // Simple as Salt
            1686, // Plumbing the Past
            1687, // What's in the Air
            1665, // An Unstable Foundation
            1664, // Explosive Progress
            1663, // One Man's Rock
            1667, // Just in Lime
            1666, // The Gall on That Giant
            1668, // To Boldly Gall
            1670, // A Rock After My Own Heart
            1671, // March for Magitek
            1669, // Training Up
            1674, // Enriching the Soil
            1673, // Sand in My Boots
            1672, // Stone Cold
            1677, // Reactionary Reactors
            1675, // Rocks of a Feather
            1676, // The Final Touch
        ],
        // Br'uk Ts'on
        [1048392] = [
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
