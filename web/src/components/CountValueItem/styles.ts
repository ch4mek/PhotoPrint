import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const CountValueWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	width: 'fit-content',
	gap: theme.spaces.xs,
	color: theme.palette['text-gray'],
}));

export const CountValueTitle = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
}));

export const CountValueText = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
}));
