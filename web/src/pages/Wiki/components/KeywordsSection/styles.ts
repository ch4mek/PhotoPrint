import styled from '@emotion/styled';

export const KeywordsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexWrap: 'wrap',
	gap: theme.spaces.s,
	color: theme.palette['text-gray'],
	width: '100%',
	marginTop: theme.spaces.xl,
	marginBottom: theme.spaces.l,

	whiteSpace: 'normal',
	wordBreak: 'break-all',
}));
