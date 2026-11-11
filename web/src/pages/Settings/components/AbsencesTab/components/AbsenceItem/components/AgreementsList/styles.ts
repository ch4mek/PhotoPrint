import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const AgreementItemWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	paddingLeft: theme.spaces.s,
	gap: theme.spaces.s,
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
}));

export const AgreementName = styled(Typography)(({ theme }) => ({
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	color: theme.palette['text-black'],
}));

export const AgreementInfoSection = styled('div')(({ theme }) => ({
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	display: 'flex',
	flexDirection: 'column',
	color: theme.palette['text-black'],
}));

export const AmountUsersWithAgreements = styled(Typography)(
	({ theme, onClick }) => ({
		color: theme.palette['text-gray'],

		...(onClick && {
			textDecoration: 'underline',
			cursor: 'pointer',
		}),
	})
);
