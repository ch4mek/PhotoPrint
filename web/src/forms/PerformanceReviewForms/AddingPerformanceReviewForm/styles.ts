import styled from '@emotion/styled';

const FIELDS_WRAPPER_WIDTH = '100%';

export const FieldsWrapper = styled('div')(({ theme }) => ({
	width: FIELDS_WRAPPER_WIDTH,
	display: 'flex',
	flexDirection: 'column',
	rowGap: theme.spaces.l,
}));
