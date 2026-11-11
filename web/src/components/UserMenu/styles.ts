import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';
import { Menu } from '@mui/material';

import {
	USER_MENU_TOP_MARGIN,
	USER_MENU_WIDTH,
	USER_MENU_SCROLL_MARGIN,
} from './constants';

export const UserMenuWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
	textAlign: 'center',
});

export const CustomUserMenu = styled(Menu)(() => ({
	'& .MuiPaper-root': {
		width: USER_MENU_WIDTH,
		marginTop: USER_MENU_TOP_MARGIN,

		'::-webkit-scrollbar-track-piece:vertical': {
			'&:start': {
				marginTop: USER_MENU_SCROLL_MARGIN,
			},
			'&:end': {
				marginBottom: USER_MENU_SCROLL_MARGIN,
			},
		},
	},
}));

export const UserAvatar = styled(Avatar)(({ theme }) => ({
	...theme.typography.body['s-700'],
}));
