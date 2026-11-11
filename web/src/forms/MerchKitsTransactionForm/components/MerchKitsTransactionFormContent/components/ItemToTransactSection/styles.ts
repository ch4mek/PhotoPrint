import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const ITEM_NAME_TYPOGRAPHY_HEIGHT = 40;

export const SizeSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',

	gap: theme.spaces.s,
	maxWidth: '100%',
	width: '100%',
}));

export const SizeWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	flexWrap: 'wrap',
	maxWidth: '100%',
}));

export const SizeItemWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const InfoItemSection = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
}));

export const NameWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	height: ITEM_NAME_TYPOGRAPHY_HEIGHT,
	borderRadius: BorderRadiuses.m,
	background: theme.palette['background-base-2'],
	alignItems: 'center',
	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,
}));

export const ItemNameTypography = styled(Typography)({
	flexDirection: 'column',
	justifyContent: 'center',
	whiteSpace: 'nowrap',
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	textAlign: 'left',
	width: '100%',
});

export const AvailableSizes = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
}));
