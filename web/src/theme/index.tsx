import {
	darkPalette,
	lightPalette,
	spaces,
	typography,
	breakpoints,
} from '@mms/mms-ui-library/theme';
import { ThemeOptions } from '@mui/material';
import { grey, red } from '@mui/material/colors';
import { RefAttributes } from 'react';
import { Link as RouterLink } from 'react-router-dom';

function LinkBehavior(
	props: { href: string } & RefAttributes<HTMLAnchorElement>
) {
	const { href, ref, ...other } = props;

	return <RouterLink ref={ref} to={href} {...other} />;
}

export enum ThemeMode {
	DARK = 'dark',
	LIGHT = 'light',
}

export const WHITE = '#FFFFFF';
export const BLACK = '#000000';
export const ERROR = '#FF0221';
export const SUCCESS = '#20BE79';
export const INFO = '#0088D1';
export const SECONDARY = '#F17900';
export const MENU_DARK_COLOR = '#494D5B';

export const RED_TAG_COLOR = '#FBB3B3';
export const YELLOW_TAG_COLOR = '#FBF7B3';
export const ORANGE_TAG_COLOR = '#FBD7B3';
export const GREEN_TAG_COLOR = '#B9E7CF';
export const GREY_TAG_COLOR = '#EEEEEE';

export enum TotalColumnColor {
	DARK = '#383C49',
	LIGHT = '#F6F7F9',
}

export const getDesignTokens = (mode: ThemeMode) =>
	({
		mode,
		spaces,
		breakpoints,
		typography: {
			sizes: [16, 24, 46],
			...typography,
		},
		customShadows: [
			'0px 2px 4px -1px rgb(0 0 0 / 20%), 0px 4px 5px 0px rgb(0 0 0 / 14%), 0px 1px 10px 0px rgb(0 0 0 / 12%)',
			'0px 5px 5px -3px rgb(0 0 0 / 20%), 0px 8px 10px 1px rgb(0 0 0 / 14%), 0px 3px 14px 2px rgb(0 0 0 / 12%)',
			'0px 1px 10px -1px rgba(34, 60, 80, 0.2)',
			'0px 0px 12px rgba(0, 0, 0, 0.26)',
			'0px 0px 0px -2px rgba(0,0,0,0.2), 0px 0px 2px 0px rgba(0,0,0,0.14), 0px 1px 5px 0px rgba(0,0,0,0.12)',
			'0px 0px 12px rgba(0, 0, 0, 0.1)',
		],
		namedShadows: {
			bookBtnTooltip: '0px 0px 17px 1px rgba(0, 0, 0, 0.12)',
			contentBlock: '0px 0px 12px rgba(0, 0, 0, 0.1)',
			bookListWrapper: '-6px 0px 6px rgba(0, 0, 0, 0.1)',
			chartTooltip: '0px 0px 3px rgba(0, 0, 0, 0.25)',
			filtersPopover:
				mode === ThemeMode.LIGHT
					? '0px 2px 30px 0px rgba(123, 140, 163, 0.25)'
					: '0px 2px 47px rgba(44, 47, 58, 0.8)',
			dropDownListWrapper:
				mode === ThemeMode.LIGHT
					? '0px 2px 30px rgba(113, 121, 155, 0.2)'
					: '0px 2px 47px rgba(44, 47, 58, 0.80)',
			form:
				mode === ThemeMode.LIGHT
					? '0 2px 30px 0 rgba(123, 140, 163, 0.25)'
					: '0px 2px 47px rgba(44, 47, 58, 0.80)',
			headerPopover:
				mode === ThemeMode.DARK
					? '0px 2px 47px 0px rgba(44, 47, 58, 0.8)'
					: '0px 2px 30px 0px rgba(113, 121, 155, 0.2)',
		},
		palette: {
			...(mode === ThemeMode.LIGHT ? lightPalette : darkPalette),
			mode,
			...(mode === ThemeMode.LIGHT
				? {
						primary: {
							main: '#2C3144',
							contrastText: WHITE,
						},
						secondary: {
							main: '#F17900',
							contrastText: WHITE,
							hover: '#2c31441f',
							additional: '#e5e6e8',
						},
						cancel: {
							main: '#636674',
							contrastText: WHITE,
						},
						background: {
							box: WHITE,
						},
						error: {
							main: red.A400,
						},
						divider: grey[200],
						text: {
							primary: grey[900],
							secondary: grey[800],
						},
						tableCells: {
							main: 'rgba(44, 49, 68, 0.1)',
							mainHover: 'rgba(5, 5, 5, 0.238)',
							innerHover: 'rgba(44, 49, 68, 0.04)',
							header: '#2C3144',
							background: WHITE,
						},
						timeSheets: {
							main: '#C4C4C4',
							mainHover: BLACK,
							backgroundLight: WHITE,
							backgroundDark: WHITE,
							totalText: WHITE,
						},
						buttons: {
							main: '#373737',
							secondary: '#636674',
							color: '#373737',
							additional: '#909090',
							active: '#6B92B1',
							exportMain: '#2C3144',
							exportHover: '#636674',
						},
						icons: {
							primary: '#373737',
							activeMenuItem: WHITE,
							mainMenu: '#909090',
							iconBox: WHITE,
						},
						table: {
							main: '#F3F3F3',
							secondary: '#E4E4E4',
							headColor: '#909090',
							rowBorder: '#F3F3F3',
							nestedTable: '#FAFAFA',
							divider: '#E4E4E4',
							border: '#F3F3F3',
						},
						autocomplete: {
							main: 'rgba(255, 196, 128, 0.27)',
							secondary: '#FFC480',
							mainHover: 'rgba(0, 0, 0, 0.12)',
							category: '#C0C0C0',
							scrollbarThumb: '#FF9326',
						},
						timeCell: {
							main: '#373737',
							secondary: '#909090',
						},
						pagination: {
							main: '#373737',
							secondary: '#909090',
						},
						pageHeader: {
							main: '#2C3144',
							secondary: WHITE,
						},
						mainMenu: {
							main: '#2C3144',
							drawer: '#3C425A',
							items: '#F8F8F8',
						},
						scrollBar: {
							track: '#E7E7E7',
							thumb: '#F17900',
						},
						field: {
							main: '#373737',
							secondary: '#909090',
							label: WHITE,
							hover: BLACK,
							focus: BLACK,
							arrow: '#373737',
						},
						form: {
							background: WHITE,
							tabColor: '#2C3144',
							scrollThumb: '#C7C7C7',
							headerIconColor: '#FFC480',
							headerTypography: WHITE,
						},
						editIcons: {
							main: '#F17900',
							delete: '#FF0221',
							cross: '#2C3144',
						},
						tooltip: {
							main: '#383D4C',
							background: WHITE,
							secondary: BLACK,
						},
						barChart: {
							secondary: '#2C3144',
							divider: '#F3F3F3',
							gridLines: '#F3F3F3',
						},
						pieChart: {
							main: '#373737',
							secondary: '#878787',
						},
						scroll: {
							main: '#F17900',
							secondary: '#E7E7E7',
						},
						formattedTime: {
							main: '#373737',
							secondary: '#909090',
						},
						banner: {
							main: '#20BE79',
						},
						switch: {
							track: '#CCCCCC',
							trackActive: '#F17900',
							thumb: WHITE,
						},
						bookingWorkplace: {
							main: '#5170BD',
							available: '#E9E9E9',
							blocked: '#5170BD',
							booked: '#4AF05E',
							selected: '#F17900',
							borderColor: WHITE,
							title: {
								main: '#ABABAB',
								blocked: '#677fb8',
								booked: '#2b9337',
								selected: '#df852b',
							},
						},
						warning: {
							main: WHITE,
							backgroundColor: '#FFC82C',
						},
						toast: {
							icon: '#2C3144',
							backgroundColor: '#000',
							title: BLACK,
							message: '#565656',
						},
						trackedTimeCell: {
							info: '#4200FF',
						},
				  }
				: {
						// =========================================================
						// Тёмная комната — палитра платформы «Пульс» / Фотопринт
						// Янтарный акцент, глубокий угольный фон, семантические цвета
						// =========================================================
						primary: {
							main: '#FFB020',        // янтарный — фонарь засветки
							contrastText: '#14100a',
						},
						secondary: {
							main: '#FFB020',
							contrastText: '#14100a',
							hover: 'rgba(255,176,32,0.14)',
						},
						cancel: {
							main: '#636674',
							contrastText: '#FFFFFF',
						},
						background: {
							default: '#0C0E13',
							paper: '#141824',
							body: '#0C0E13',
							box: '#1b2130',
						},
						text: {
							primary: '#EDF0F7',
							secondary: '#9AA3B8',
						},
						action: {
							active: '#EDF0F7',
							selectedOpacity: 0.8,
						},
						error: {
							main: '#FF5C5C',
						},
						success: {
							main: '#3AD97E',
						},
						tableCells: {
							main: 'rgba(255,176,32,0.08)',
							mainHover: 'rgba(255,176,32,0.14)',
							innerHover: 'rgba(255,176,32,0.04)',
							header: '#EDF0F7',
							background: '#1b2130',
						},
						timeSheets: {
							main: '#232a3d',
							mainHover: '#EDF0F7',
							backgroundLight: '#141824',
							backgroundDark: '#0C0E13',
							totalText: '#14100a',
						},
						buttons: {
							main: '#9AA3B8',
							secondary: '#9AA3B8',
							color: '#EDF0F7',
							additional: '#9AA3B8',
							active: '#FFB020',
							exportMain: '#141824',
							exportHover: '#232a3d',
						},
						icons: {
							primary: '#EDF0F7',
							activeMenuItem: '#FFB020',
							mainMenu: '#9AA3B8',
							iconBox: 'rgba(255,255,255,0.08)',
						},
						table: {
							main: '#1b2130',
							secondary: '#232a3d',
							headColor: '#9AA3B8',
							rowBorder: '#272e40',
							nestedTable: '#141824',
							divider: '#272e40',
							border: '#272e40',
						},
						autocomplete: {
							main: 'rgba(255,176,32,0.14)',
							secondary: '#FFB020',
							mainHover: 'rgba(255,176,32,0.22)',
							category: '#9AA3B8',
							scrollbarThumb: '#FFB020',
						},
						timeCell: {
							main: '#EDF0F7',
							secondary: '#9AA3B8',
						},
						pagination: {
							main: '#9AA3B8',
							secondary: '#9AA3B8',
						},
						pageHeader: {
							main: '#EDF0F7',
							secondary: '#EDF0F7',
						},
						mainMenu: {
							main: '#08090d',
							drawer: 'rgba(255,176,32,0.08)',
							items: 'rgba(255,176,32,0.08)',
						},
						scrollBar: {
							track: '#1b2130',
							thumb: '#FFB020',
						},
						field: {
							main: '#9AA3B8',
							secondary: '#9AA3B8',
							label: '#7a8499',
							hover: '#EDF0F7',
							focus: '#FFB020',
							arrow: '#EDF0F7',
						},
						form: {
							background: '#141824',
							tabColor: '#EDF0F7',
							scrollThumb: '#232a3d',
							headerIconColor: '#FFB020',
							headerTypography: '#EDF0F7',
						},
						editIcons: {
							main: '#FFB020',
							delete: '#FF5C5C',
							cross: '#9AA3B8',
						},
						tooltip: {
							main: '#141824',
							background: '#1b2130',
							secondary: '#EDF0F7',
						},
						barChart: {
							secondary: '#EDF0F7',
							divider: '#272e40',
							gridLines: '#1b2130',
						},
						pieChart: {
							main: '#EDF0F7',
							secondary: '#9AA3B8',
						},
						scroll: {
							main: '#FFB020',
							secondary: '#1b2130',
						},
						formattedTime: {
							main: '#EDF0F7',
							secondary: '#9AA3B8',
						},
						banner: {
							main: '#3AD97E',
						},
						switch: {
							track: '#272e40',
							trackActive: '#FFB020',
							thumb: '#EDF0F7',
						},
						bookingWorkplace: {
							main: '#5DA9FF',
							available: '#9AA3B8',
							blocked: '#5DA9FF',
							booked: '#3AD97E',
							selected: '#FFB020',
							borderColor: '#0C0E13',
							title: {
								main: '#0C0E13',
								blocked: '#5DA9FF',
								booked: '#2b9337',
								selected: '#df852b',
							},
						},
						warning: {
							main: '#EDF0F7',
							backgroundColor: '#FFB020',
						},
						toast: {
							icon: '#FFB020',
							backgroundColor: '#141824',
							title: '#EDF0F7',
							message: '#9AA3B8',
						},
						trackedTimeCell: {
							info: '#A78BFA',
						},
				  }),
		},
		components: {
			MuiLink: {
				defaultProps: {
					component: LinkBehavior,
				},
			},
			MuiButtonBase: {
				defaultProps: {
					LinkComponent: LinkBehavior,
				},
			},
		},
	} as unknown as ThemeOptions);
