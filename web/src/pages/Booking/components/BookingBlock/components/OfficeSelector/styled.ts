import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { lightPalette } from '@mms/mms-ui-library/theme';

import {
	DEFAULT_ICON_SIZE,
	PLAIN_TEXT_FONT_SIZE,
} from '@/constants/commonStyles';

export const OfficeSelectorChevronWrapper = styled('div')<{ isOpen: boolean }>(
	({ isOpen }) => ({
		transform: isOpen ? 'none' : 'rotate(180deg)',
		width: DEFAULT_ICON_SIZE,
		height: DEFAULT_ICON_SIZE,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
	})
);

export const OfficeSelectorWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	cursor: 'pointer',
}));

export const OfficeName = styled(Typography)(({ theme }) => ({
	color: lightPalette['text-white'],

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		fontSize: PLAIN_TEXT_FONT_SIZE,
	},
}));
