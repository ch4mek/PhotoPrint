import { ThemeMode } from '@/theme';

import darkThemeMapFloor4Image from './assets/DarkThemeMapFloor4Image.svg';
import mapFloor4Image from './assets/MapFloor4Image.svg';

export const POPUP_WIDTH_VW = 90;
export const OFFICE_MAP_WIDTH_PX = 1361;
export const OFFICE_MAP_HEIGHT_PX = 448;

export const popUpStylesConfig = {
	container: {
		width: `${POPUP_WIDTH_VW}vw`,
		display: 'flex',
		flexDirection: 'column',

		'& > header': {
			flexShrink: 0,
		},

		'& > div': {
			flexGrow: 1,
			minHeight: 0,
			display: 'flex',
		},
	},
	content: {
		display: 'flex',
		flexGrow: 1,

		height: 'auto',
		padding: 0,
	},
};

export const mapTabsConfig = [
	// TODO: uncomment within MMS2-6978
	// {
	// 	id: 2,
	// 	text: 'map.floors.second',
	// 	image: {
	// 		[ThemeMode.LIGHT]: mapFloor2Image,
	// 		[ThemeMode.DARK]: darkThemeMapFloor2Image,
	// 	},
	// },
	{
		id: 4,
		text: 'map.floors.fourth',
		image: {
			[ThemeMode.LIGHT]: mapFloor4Image,
			[ThemeMode.DARK]: darkThemeMapFloor4Image,
		},
	},
] as const;
