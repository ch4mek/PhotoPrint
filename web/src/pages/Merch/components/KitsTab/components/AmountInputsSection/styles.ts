import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const MERCH_DESKTOP_BREAKPOINT = 832;
const MERCH_MOBILE_BREAKPOINT = 500;
const MIN_STORAGE_ITEM_WIDTH = 50;
const SIZE_ITEM_WIDTH = 71;

export const AmountSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	flexWrap: 'wrap',
	gap: theme.spaces.l,
	...theme.typography.body['m-400'],
}));

export const AmountPerStorageSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
}));

export const StorageWrapper = styled(Typography)(({ theme }) => ({
	display: 'flex',
	maxWidth: MIN_STORAGE_ITEM_WIDTH,
	minWidth: 'auto',
	alignItems: 'center',

	[theme.breakpoints.down(MERCH_DESKTOP_BREAKPOINT)]: {
		alignItems: 'center',
	},

	[theme.breakpoints.down(MERCH_MOBILE_BREAKPOINT)]: {
		minWidth: MIN_STORAGE_ITEM_WIDTH,
	},
}));

export const AmountInputWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	gap: 0,
	width: SIZE_ITEM_WIDTH,
});

export const AmountInputsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	flexWrap: 'wrap',
}));
