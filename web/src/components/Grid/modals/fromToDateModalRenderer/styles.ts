import styled from '@emotion/styled';

import { MODAL_WIDTH } from '../../constants';

const MODAL_HEIGHT = 220;

export const FromToDateModalInputsWrapper = styled('div')();

export const DataWrapper = styled('div')<{ bottomSpace: number }>(
	({ bottomSpace }) => ({
		marginBottom: bottomSpace,
	})
);

export const FromToDateModalWrapper = styled('div')(({ theme }) => ({
	width: MODAL_WIDTH,
	height: MODAL_HEIGHT,
	display: 'flex',
	flexDirection: 'column',
	padding: theme.spaces.m,
	gap: theme.spaces.m,
}));
