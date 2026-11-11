import styled from '@emotion/styled';

const MIN_TITLE_HEIGHT = 36;

export const HomeHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	alignItems: 'flex-start',
	justifyContent: 'space-between',
	gap: theme.spaces.xl2,
}));

export const PageTitleWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
	minHeight: MIN_TITLE_HEIGHT,
});

export const PageTitleContent = styled('div')(({ theme }) => ({
	wordBreak: 'break-word',
	whiteSpace: 'normal',
	position: 'relative',

	'& svg': {
		position: 'absolute',
		marginLeft: theme.spaces.xs,
	},
}));
