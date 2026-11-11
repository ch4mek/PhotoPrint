import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { MOBILE_VIEW_REQUEST_FORM_BREAKPOINT } from '@/forms/Absence/ViewAbsenceRequestForm/constants';

export const RequestHeaderInformationWrapper = styled('div')(({ theme }) => ({
	flex: 1,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,

	[theme.breakpoints.down(MOBILE_VIEW_REQUEST_FORM_BREAKPOINT)]: {
		width: '100%',
	},
}));

export const RequestHeaderInfoWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const Label = styled(Typography)({
	display: '-webkit-box',
	overflowWrap: 'break-word',
	wordBreak: 'break-word',
	whiteSpace: 'normal',
	textOverflow: 'ellipsis',
	overflow: 'hidden',
});

export const RequestHeaderDatesWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	alignItems: 'center',
	flexWrap: 'wrap',
	color: theme.palette['text-gray'],
}));
