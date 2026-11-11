import styled from '@emotion/styled';
import { CSSProperties } from 'react';

export const checkboxWrapperStyles: CSSProperties = {
	alignItems: 'flex-start',
};

export const FormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));
