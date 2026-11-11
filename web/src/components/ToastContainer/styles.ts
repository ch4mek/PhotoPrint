import styled from '@emotion/styled';
import { ToastContainer } from 'react-toastify';

import { TOAST_MIN_HEIGHT, TOAST_WIDTH } from './constants';
import type { StyledToastContainerProps } from './types';

export const ToastContainerWrapper = styled(
	ToastContainer
)<StyledToastContainerProps>(({ styles }) => ({
	bottom: styles.spaces.m,
	right: styles.spaces.m,
	padding: 0,
	display: 'flex',
	flexDirection: 'column',
	gap: styles.spaces.s,

	'& .Toastify__toast': {
		padding: 0,
		width: TOAST_WIDTH,
		minHeight: TOAST_MIN_HEIGHT,
		marginBottom: 0,
		transform: 'none',
		boxShadow: 'none',
	},

	'& .Toastify__toast-body': {
		padding: 0,
	},
}));
