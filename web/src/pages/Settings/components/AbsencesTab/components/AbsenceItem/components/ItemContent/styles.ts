import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const ItemContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,

	color: theme.palette['text-black'],

	flex: 1,
	minWidth: 0,
}));

export const ItemDescription = styled('div')({
	display: 'flex',
	flexDirection: 'row',
	alignItems: 'center',
	flexWrap: 'wrap',
});

export const DescriptionTypography = styled(Typography)(({ onClick }) => ({
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',

	...(onClick && {
		textDecoration: 'underline',
		cursor: 'pointer',
	}),
}));
