import styled from '@emotion/styled';

const HEADER_HEIGHT = 57;

export const ChatContentWrapper = styled('div')(({ theme }) => ({
	flex: 1,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,

	width: '100%',
	height: `calc(100% - ${HEADER_HEIGHT}px)`,
}));
