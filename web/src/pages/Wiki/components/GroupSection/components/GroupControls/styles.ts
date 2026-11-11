import styled from '@emotion/styled';

const MIN_INPUT_WIDTH = 150;

export const GroupControlsWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	gap: theme.spaces.s,
	padding: theme.spaces.m,
	paddingBottom: 0,

	flexWrap: 'wrap',
}));

export const SearchInputWrapper = styled('div')({
	flex: 1,
	minWidth: MIN_INPUT_WIDTH,
});
