import styled from '@emotion/styled';

const MODAL_WIDTH = 244;

export const ModalWrapper = styled('div')(({ theme }) => ({
	width: MODAL_WIDTH,
	padding: theme.spaces.m,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
}));
