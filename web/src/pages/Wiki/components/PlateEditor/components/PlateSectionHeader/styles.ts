import styled from '@emotion/styled';

export const PlateHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'start',
	gap: theme.spaces.s,
	justifyContent: 'space-between',
	padding: theme.spaces.l,
	paddingBottom: 0,
	width: '100%',

	marginBottom: theme.spaces.s,
}));

export const PlateControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
}));

export const PlateTitleWrapper = styled('div')({
	flex: 1,

	whiteSpace: 'normal',
	wordBreak: 'break-word',
});
