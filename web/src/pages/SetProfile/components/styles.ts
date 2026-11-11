import styled from '@emotion/styled';
import { Form } from 'formik';

import {
	PROFILE_MOBILE_S,
	PROFILE_MOBILE_XS,
} from '@/components/ProfileAvatar/constants';
import { BorderRadiuses } from '@/constants/borderRadiuses';

import {
	PROFILE_WRAPPER_MAX_HEIGHT,
	PROFILE_WRAPPER_MAX_WIDTH_MOBILE_S,
} from './constants';

export const AuthFormWrapper = styled(Form)(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'center',
	width: '100%',
	gap: theme.spaces.l,
	alignItems: 'center',
	position: 'relative',
}));

export const Wrapper = styled('div')(({ theme }) => ({
	borderRadius: BorderRadiuses.m,
	padding: theme.spaces.s,
	maxHeight: PROFILE_WRAPPER_MAX_HEIGHT,
	overflowX: 'hidden',

	[theme.breakpoints.down(PROFILE_MOBILE_S)]: {
		maxWidth: PROFILE_WRAPPER_MAX_WIDTH_MOBILE_S,
		maxHeight: 'max-content',
	},

	[theme.breakpoints.down(PROFILE_MOBILE_XS)]: {
		width: '100%',
		padding: theme.spaces.m,
	},
}));
