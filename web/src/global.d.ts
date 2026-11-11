import {
	lightTheme,
	spaces,
	typography,
	breakpoints,
} from '@mms/mms-ui-library/theme';
import {
	Palette,
	Breakpoints,
	Theme as MuiTheme,
	PaletteColorOptions,
} from '@mui/material';
import '@emotion/react';

import '@mui/material/styles';
import { ThemeMode } from './theme';

type UIKitPalette = typeof lightTheme.palette;

interface OverrideOptions extends UIKitPalette {
	pageHeader?: Palette['main'];
	pieChart?: Palette['main'];
	pieChart?: Palette['secondary'];
	buttons?: Palette['main'];
	buttons?: Palette['secondary'];
	tableCells?: Palette['main'];
	mainMenu?: Palette['main'];
	pagination?: Palette['main'];
	form?: Palette['main'];
	table?: Palette['main'];
	field?: Palette['main'];
	timeCell?: Palette['main'];
	tooltip?: Palette['main'];
	scrollBar?: Palette['main'];
	icons?: Palette['main'];
	pageHeader?: Palette['main'];
	formattedTime?: Palette['main'];
	formattedTime?: Palette['secondary'];
	barChart?: Palette['secondary'];
	barChart?: Palette['divider'];
	barChart?: Palette['gridLines'];
	scroll?: Palette['main'];
	scroll?: Palette['secondary'];
	table?: Palette['headColor'];
	table?: Palette['divider'];
	table?: Palette['border'];
	tableCells?: Palette['main'];
	buttons?: Palette['main'];
	buttons?: Palette['secondary'];
	cancel?: Palette['main'];
	form?: Palette['headerIconColor'];
	form?: Palette['background'];
	form?: Palette['headerTypography'];
	form?: Palette['tabColor'];
	field?: Palette['main'];
	field?: Palette['secondary'];
	field?: Palette['hover'];
	field?: Palette['focus'];
	autocomplete?: Palette['scrollbarThumb'];
	timeSheets?: Palette['backgroundLight'];
	scrollBar?: Palette['track'];
	editIcons?: Palette['delete'];
	bookingWorkplace?: Palette['available'];
	warning?: Palette['main'];
	warning?: Palette['backgroundColor'];
	switch?: Palette['track'];
	switch?: Palette['thumb'];
	switch?: Palette['trackActive'];
	toast?: Palette['backgroundColor'];
	toast?: Palette['icon'];
	trackedTimeCell?: Palette['info'];
}

interface NamedShadows {
	bookBtnTooltip: string;
	contentBlock: string;
	bookListWrapper: string;
	dropDownListWrapper: string;
	chartTooltip: string;
	filtersPopover: string;
	form: string;
	headerPopover: string;
}

declare module '@mui/material/styles' {
	interface Palette extends OverrideOptions {
		pageHeader?: Palette['main'];
		bookingWorkplace: Palette['available'];
		barChart?: Palette['gridLines'];
	}

	interface PaletteOptions extends OverrideOptions {
		pieChart?: Palette['main'];
		pageHeader?: Palette['main'];
		bookingWorkplace: PaletteColorOptions;
	}

	interface PaletteColor {
		main?: string;
		secondary?: string;
		divider?: string;
		border?: string;
		backgroundColor?: string;
		available?: string;
		info?: string;
	}

	interface TypeBackground {
		box: string;
	}

	interface TypographyVariants {
		sizes: Array<number>;
		fontFamily: string;
	}

	interface TypographyVariantsOptions {
		sizes: Array<number>;
		fontFamily: string;
	}

	interface ThemeOptions {
		customShadows: Array<string>;
		namedShadows: NamedShadows;
		breakpoints: typeof breakpoints;
		palette: PaletteOptions;
	}

	interface Theme {
		mode: ThemeMode;
		customShadows: string[];
		namedShadows: NamedShadows;
		spaces: typeof spaces;
		breakpoints: typeof breakpoints & Breakpoints;
		typography: typeof typography & {
			pxToRem: (value: number) => string;
			sizes: string[];
			fontWeightMedium: string | number;
			fontWeightBold: string | number;
		};
	}
}

declare module '@emotion/react' {
	interface Theme extends MuiTheme {}
}

declare module '@mui/material/Badge' {
	interface BadgePropsColorOverrides {
		bookingWorkplace: true;
	}
}

declare global {
	interface Window {
		logout: (reason?: string) => void;
		cancelLogout: () => void;
	}

	interface FederatedContainer {
		init(shareScope: Record<string, unknown>): Promise<void>;
		get: (module: string) => Promise<() => unknown>;
	}
}

declare module 'recharts' {
	type RadiusType = [number, number, number, number];

	interface CellProps {
		key?: React.Key;
		radius?: RadiusType;
		fill?: string;
		children?: ReactNode;
	}

	const Cell: React.ComponentType<CellProps>;
}
