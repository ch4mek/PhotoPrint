import { breakpoints, spaces } from '@mms/mms-ui-library/theme';

import blueEggImage from '@/assets/events/blueEgg.png';
import blueTankIcon from '@/assets/events/blueTank.png';
import christmasHatIcon from '@/assets/events/christmasHat.png';
import easterCakeImage from '@/assets/events/easterCake.png';
import flowerOrangeImage from '@/assets/events/flowerOrange.png';
import flowerRedImage from '@/assets/events/flowerRed.png';
import flowerWhiteImage from '@/assets/events/flowerWhite.png';
import footballImage1 from '@/assets/events/football-1st-part.webp';
import footballImage2 from '@/assets/events/football-2nd-part.webp';
import footballImage3 from '@/assets/events/football-3rd-part.webp';
import footballImageBg from '@/assets/events/football-bg.webp';
import lightBlueTankIcon from '@/assets/events/lightBlueTank.png';
import orangeEggImage from '@/assets/events/orangeEgg.png';
import orangeHeartIcon from '@/assets/events/orangeHeart.png';
import orangeTankIcon from '@/assets/events/orangeTank.png';
import pinkHeartIcon from '@/assets/events/pinkHeart.png';
import springImage1 from '@/assets/events/spring-1st-part.svg';
import springImage2 from '@/assets/events/spring-2nd-part.svg';
import springImage3 from '@/assets/events/spring-3rd-part.svg';
import springImage4 from '@/assets/events/spring-4th-part.svg';
import springImage5 from '@/assets/events/spring-5th-part.svg';
import springImage6 from '@/assets/events/spring-6th-part.svg';
import { EventParticlesConfig } from '@/types/events';

const valentinesDayEventConfig: EventParticlesConfig = {
	name: 'valentinesDay',
	particles: {
		count: 30,
		speed: [0.3, 0.7],
		wind: [0, 0],
		size: [5, 10],
		opacity: [0.2, 1],
		rotationSpeed: [-0.5, 0.5],
		initialRotation: [0, 0],
		images: [orangeHeartIcon, pinkHeartIcon],
	},
	schedule: {
		startDate: '2025-02-13T00:00:00+03:00',
		endDate: '2025-02-15T00:00:00+03:00',
	},
};

const christmasEventConfig: EventParticlesConfig = {
	name: 'christmas',
	particles: {
		count: 40,
		speed: [0.5, 1.0],
		size: [3, 5],
	},
	decorativeIcon: {
		src: christmasHatIcon,
		offsetY: -5,
		offsetX: -8,
	},
	schedule: {
		startDate: '2025-12-25T00:00:00+03:00',
		endDate: '2026-01-06T00:00:00+03:00',
	},
};

const defenderOfTheFatherlandEventConfig: EventParticlesConfig = {
	name: 'defenderOfTheFatherland',
	particles: {
		count: 20,
		speed: [0.6, 1],
		size: [6, 10],
		direction: 'left',
		initialRotation: [270, 270],
		images: [blueTankIcon, lightBlueTankIcon, orangeTankIcon],
	},
	schedule: {
		startDate: '2026-02-20T08:00:00+03:00',
		endDate: '2026-02-24T09:00:00+03:00',
	},
};

const SPRING_IMAGE_HIDE_FIRST = 1800;
const SPRING_IMAGE_HIDE_SECOND = 1520;

const beginningOfSpringEventConfig: EventParticlesConfig = {
	name: 'beginningOfSpring',
	staticDecorations: {
		placement: 'bottom',
		zonesGap: {
			desktop: spaces.xl3,
			tablet: spaces.l,
			mobile: 0,
		},
		zones: [
			{
				position: 'left',
				items: [
					{
						id: 'spring-left',
						images: {
							desktop: springImage1,
							tablet: springImage6,
						},
					},
				],
			},
			{
				position: 'center',
				contentAlign: 'flex-start',
				gap: {
					desktop: spaces.xl3,
					tablet: spaces.l,
					mobile: 0,
				},
				items: [
					{
						id: 'spring-2',
						images: { desktop: springImage2 },
					},
					{
						id: 'spring-3',
						images: { desktop: springImage3 },
						hideBelow: SPRING_IMAGE_HIDE_FIRST,
					},
					{
						id: 'spring-4',
						images: { desktop: springImage4 },
						hideBelow: SPRING_IMAGE_HIDE_SECOND,
					},
				],
				hideBelow: breakpoints.desktopS,
			},
			{
				position: 'right',
				items: [
					{
						id: 'spring-right',
						images: { desktop: springImage5 },
					},
				],
			},
		],
	},
	schedule: {
		startDate: '2026-03-01T08:00:00+03:00',
		endDate: '2026-03-06T08:00:00+03:00',
	},
};

const womensDayConfig: EventParticlesConfig = {
	name: 'womensDay',
	particles: {
		count: 15,
		speed: [0.3, 0.8],
		wind: [0, 0],
		size: [4, 12],
		opacity: [0.4, 1],
		rotationSpeed: [1, 3],
		initialRotation: [0, 180],
		images: [flowerWhiteImage, flowerRedImage, flowerOrangeImage],
	},
	schedule: {
		startDate: '2026-03-06T08:00:00+03:00',
		endDate: '2026-03-09T08:00:00+03:00',
	},
};

const easterConfig: EventParticlesConfig = {
	name: 'easter',
	particles: {
		count: 150,
		speed: [0.4, 0.8],
		wind: [0, 0],
		size: [6, 14],
		opacity: [0.6, 1],
		rotationSpeed: [1, 3],
		initialRotation: [0, 90],
		images: [blueEggImage, orangeEggImage, easterCakeImage],
	},
	schedule: {
		startDate: '2026-04-06T08:00:00+03:00',
		endDate: '2026-04-14T08:00:00+03:00',
	},
};

const FOOTBALL_HEADER_BREAKPOINT_LARGE = 1600;
const FOOTBALL_HEADER_BREAKPOINT_MEDIUM = 970;

const FOOTBALL_HEADER_BACKGROUND =
	'radial-gradient(72.7% 220.37% at 50% 146.3%, rgba(125, 187, 210, 0.3) 0%, rgba(44, 49, 68, 0) 100%), #2C3144';

const fifaWorldCupEventConfig: EventParticlesConfig = {
	name: 'fifaWorldCup',
	headerBackground: FOOTBALL_HEADER_BACKGROUND,
	staticDecorations: {
		placement: 'bottom',
		zonesGap: {
			desktop: 0,
			tablet: 0,
			mobile: 0,
		},
		zones: [
			{
				position: 'left',
				items: [
					{
						id: 'football-left',
						images: {
							desktop: footballImage1,
							[FOOTBALL_HEADER_BREAKPOINT_MEDIUM]: footballImage2,
						},
						hideBelow: breakpoints.tabletS,
					},
				],
				styles: {
					desktop: {
						marginLeft: 146,
						width: 676,
					},
					[FOOTBALL_HEADER_BREAKPOINT_LARGE]: {
						marginLeft: 48,
					},
					[FOOTBALL_HEADER_BREAKPOINT_MEDIUM]: {
						marginLeft: 51,
						width: 324,
					},
				},
			},
			{
				position: 'center',
				items: [
					{
						id: 'football-bg',
						images: {
							desktop: footballImageBg,
						},
					},
				],
				styles: {
					desktop: {
						position: 'absolute',
						transform: 'rotate(38.5deg)',
						marginLeft: 260,
						top: -187,
						opacity: 0.1,
						width: 478,
						zIndex: -1,
					},
					[FOOTBALL_HEADER_BREAKPOINT_LARGE]: {
						marginLeft: 162,
					},
					[FOOTBALL_HEADER_BREAKPOINT_MEDIUM]: {
						marginLeft: -162,
					},
				},
			},
			{
				position: 'right',
				items: [
					{
						id: 'football-right',
						images: { desktop: footballImage3 },
					},
				],
				styles: {
					desktop: {
						position: 'absolute',
						right: 278,
						width: 694,
					},
				},
				hideBelow: FOOTBALL_HEADER_BREAKPOINT_LARGE,
			},
		],
	},
	schedule: {
		startDate: '2026-07-11T08:00:00+03:00',
		endDate: '2026-07-20T13:00:00+03:00',
	},
};

export const EVENTS_PARTICLES_CONFIG: EventParticlesConfig[] = [
	valentinesDayEventConfig,
	christmasEventConfig,
	defenderOfTheFatherlandEventConfig,
	beginningOfSpringEventConfig,
	womensDayConfig,
	easterConfig,
	fifaWorldCupEventConfig,
];
