import styled from '@emotion/styled';

import { EQUIPMENT_USER_FORM_MAX_WIDTH } from '../../constants';

export const FormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
	width: EQUIPMENT_USER_FORM_MAX_WIDTH,
	maxWidth: '100%',
}));

export const DatePickerWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	gap: theme.spaces.s,
	flexDirection: 'row',
	justifyContent: 'space-between',
}));
