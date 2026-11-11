import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const ControlBarTitle = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
}));

export const ControlBarWrapper = styled('div')(({ theme }) => ({
	gap: theme.spaces.s,
	display: 'flex',
	alignItems: 'center',
}));
