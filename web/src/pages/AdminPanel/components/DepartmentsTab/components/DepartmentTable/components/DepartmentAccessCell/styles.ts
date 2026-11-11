import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const AccessText = styled(Typography)(({ theme }) => ({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	color: theme.palette['text-black'],
	textDecoration: 'none',
}));
