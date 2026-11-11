import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const MERCH_DESKTOP_BREAKPOINT = 832;
const MERCH_MOBILE_BREAKPOINT = 500;
const MIN_STORAGE_ITEM_WIDTH = 50;
const SIZE_ITEM_WIDTH = 71;

export const SizeSectionWrapper = styled('div')<{
	isClothing: boolean | null;
}>(({ theme, isClothing }) => ({
	display: 'flex',
	flexDirection: isClothing ? 'column' : 'row',
	flexWrap: 'wrap',
	gap: isClothing ? theme.spaces.m : theme.spaces.l,
	...theme.typography.body['m-400'],
}));

export const SizePerStorageSectionWrapper = styled('div')<{
	isClothing: boolean | null;
}>(({ theme, isClothing }) => ({
	display: 'flex',
	gap: isClothing ? theme.spaces.m : theme.spaces.s,
}));

export const StorageWrapper = styled(Typography)<{
	isClothing: boolean | null;
}>(({ theme, isClothing }) => ({
	display: 'flex',
	maxWidth: MIN_STORAGE_ITEM_WIDTH,
	minWidth: isClothing ? MIN_STORAGE_ITEM_WIDTH : 'auto',
	alignItems: isClothing ? 'flex-end' : 'center',

	[theme.breakpoints.down(MERCH_DESKTOP_BREAKPOINT)]: {
		alignItems: isClothing ? 'flex-start' : 'center',
	},

	[theme.breakpoints.down(MERCH_MOBILE_BREAKPOINT)]: {
		minWidth: MIN_STORAGE_ITEM_WIDTH,
	},
}));

export const SizeInputWrapper = styled('div')<{
	isClothing: boolean | null;
}>(({ theme, isClothing }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: isClothing ? theme.spaces.s : 0,
	width: SIZE_ITEM_WIDTH,
}));

export const SizeIdWrapper = styled(Typography)(() => ({}));

export const SizeInputsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	flexWrap: 'wrap',
}));
