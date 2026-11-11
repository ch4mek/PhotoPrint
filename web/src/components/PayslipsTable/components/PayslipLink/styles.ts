import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const PayslipLinkTypography = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	'&:hover': {
		cursor: 'pointer',
		textDecoration: 'underline',
	},
}));
