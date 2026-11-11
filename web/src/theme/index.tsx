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
						primary: {
							main: '#F17900',
							contrastText: BLACK,
						},
						secondary: {
							main: '#F17900',
							contrastText: WHITE,
							hover: '#F17900',
						},
						cancel: {
							main: '#636674',
							contrastText: WHITE,
						},
						background: {
							default: grey[900],
							paper: grey[900],
							body: grey[900],
							box: '#353535',
						},
						text: {
							primary: WHITE,
							secondary: grey[500],
						},
						action: {
							active: WHITE,
							selectedOpacity: 0.8,
						},
						tableCells: {
							main: 'rgba(240, 240, 240, 0.2)',
							mainHover: 'rgba(240, 240, 240, 0.3)',
							innerHover: 'rgba(240, 240, 240, 0.1)',
							header: WHITE,
							background: '#353535',
						},
						timeSheets: {
							main: '#4A4A4A',
							mainHover: WHITE,
							backgroundLight: '#2C2C2C',
							backgroundDark: '#212121',
							totalText: BLACK,
						},
						buttons: {
							main: '#909090',
							secondary: '#A2A2A2',
							color: '#FFFFFF',
							additional: '#909090',
							active: '#6B92B1',
							exportMain: '#2C3144',
							exportHover: '#636674',
						},
						icons: {
							primary: WHITE,
							activeMenuItem: WHITE,
							mainMenu: '#909090',
							iconBox: 'rgba(255, 255, 255, 0.13)',
						},
						table: {
							main: '#3E3E3E',
							secondary: '#575757',
							headColor: '#909090',
							rowBorder: '#3C3C3C',
							nestedTable: '#2A2A2A',
							divider: '#575757',
							border: '#3C3C3C',
						},
						autocomplete: {
							main: 'rgba(255, 196, 128, 0.27)',
							secondary: '#FFC480',
							mainHover: 'rgba(240, 240, 240, 0.3)',
							category: '#C0C0C0',
							scrollbarThumb: '#FF9326',
						},
						timeCell: {
							main: WHITE,
							secondary: '#909090',
						},
						pagination: {
							main: '#909090',
							secondary: '#909090',
						},
						pageHeader: {
							main: WHITE,
							secondary: WHITE,
						},
						mainMenu: {
							main: '#212121',
							drawer: '#ffffff17',
							items: '#ffffff17',
						},
						scrollBar: {
							track: '#3E3E3E',
							thumb: '#F17900',
						},
						field: {
							main: '#909090',
							secondary: '#909090',
							label: '#454545',
							hover: WHITE,
							focus: '#F17900',
							arrow: WHITE,
						},
						form: {
							background: '#353535',
							tabColor: WHITE,
							scrollThumb: '#545454',
							headerIconColor: WHITE,
							headerTypography: WHITE,
						},
						editIcons: {
							main: WHITE,
							delete: '#FF0221',
							cross: '#909090',
						},
						tooltip: {
							main: '#212121',
							background: '#2D2D2D',
							secondary: WHITE,
						},
						barChart: {
							secondary: WHITE,
							divider: '#575757',
							gridLines: '#3C3C3C',
						},
						pieChart: {
							main: '#EBEBEB',
							secondary: '#C3C3C3',
						},
						scroll: {
							main: '#F17900',
							secondary: '#3E3E3E',
						},
						formattedTime: {
							main: WHITE,
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
							available: '#909090',
							blocked: '#5170BD',
							booked: '#4AF05E',
							selected: '#F17900',
							borderColor: BLACK,
							title: {
								main: BLACK,
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
							icon: WHITE,
							backgroundColor: '#353535',
							title: WHITE,
							message: '#B9B9B9',
						},
						trackedTimeCell: {
							info: '#A88AFF',
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
