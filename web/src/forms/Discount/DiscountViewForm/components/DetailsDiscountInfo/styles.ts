import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';
import { MIDDLE_BREAKPOINT } from '@/forms/Discount/DiscountViewForm/constants';
import { ThemeMode } from '@/theme';

const SECTION_WIDTH = 229;
const MIN_CONTENT_HEIGHT = 400;
const MAX_SECTION_WIDTH = 308;

export const DetailsDiscountInfoWrapper = styled('div')(({ theme }) => ({
	width: SECTION_WIDTH,
	minHeight: MIN_CONTENT_HEIGHT,

	display: 'flex',
	flexDirection: 'column',

	gap: theme.spaces.m,
	backgroundColor:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['background-primary-1']
			: theme.palette['background-primary-3'],
	border: `1px solid ${
		theme.mode === ThemeMode.LIGHT
			? theme.palette['background-primary-1']
			: theme.palette['background-primary-3']
	}`,
	borderRadius: BorderRadiuses.m,
	alignSelf: 'flex-start',

	[theme.breakpoints.down(MIDDLE_BREAKPOINT)]: {
		width: '100%',
		maxWidth: MAX_SECTION_WIDTH,
		margin: 'auto',
	},
}));

export const DetailInfoWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	flex: 1,

	display: 'flex',
	flexDirection: 'column',
	padding: `0 ${theme.spaces.m}px ${theme.spaces.m}px`,
	gap: theme.spaces.m,
}));

export const ContactsInfo = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
}));

export const ContactInfoItem = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	alignItems: 'flex-start',

	color: theme.palette['text-black'],
	...theme.typography.body['m-400'],

	'& > svg': {
		width: IconSize.MS,
		height: IconSize.MS,
		color: theme.palette['graphics-primary-2'],
	},
}));

export const CompanyTypeText = styled(Typography)({
	whitespace: 'normal',
	wordBreak: 'break-all',
});

export const DetailsInfoText = styled('div')({
	flex: 1,

	whitespace: 'normal',
	wordBreak: 'break-all',
});

export const AppLinkWrapper = styled('a')(({ theme }) => ({
	textDecoration: 'none',
	cursor: 'pointer',

	flex: 1,
	whitespace: 'normal',
	wordBreak: 'break-all',
	color: theme.palette['text-black'],
}));
