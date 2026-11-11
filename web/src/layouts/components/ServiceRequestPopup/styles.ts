import styled from '@emotion/styled';
import { Button } from '@mms/mms-ui-library/components/Button';
import { lightTheme } from '@mms/mms-ui-library/theme';

import { ThemeMode } from '@/theme';

const MENU_ITEM_HEIGHT = 40;
const MENU_BUTTON_HEIGHT = 30;

export const MenuButton = styled(Button)(({ theme }) => ({
	height: `${MENU_BUTTON_HEIGHT}px`,
	background:
		theme.mode === ThemeMode.DARK
			? lightTheme.palette['background-base-2']
			: lightTheme.palette['background-base-3'],
	color: lightTheme.palette['text-black'],
}));

export const MenuWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	backgroundColor: theme.palette['background-white'],
	borderRadius: theme.spaces.s,
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	boxShadow: theme.namedShadows.headerPopover,
	padding: theme.spaces.s,
}));

export const MenuItem = styled('button')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	height: `${MENU_ITEM_HEIGHT}px`,
	padding: theme.spaces.s,
	background: 'transparent',
	border: 'none',
	borderRadius: theme.spaces.s,
	cursor: 'pointer',
	fontSize: theme.typography.body['m-400'].fontSize,
	color: theme.palette['text-black'],
	transition: 'background-color 0.2s ease',

	'&:hover': {
		backgroundColor: theme.palette['background-base-3'],
	},

	'& svg': {
		color: theme.palette['graphics-primary-2'],
	},
}));
