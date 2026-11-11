import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { IconSize } from '@/constants/iconSizes';

const NO_PAYLOAD_DATA_MAX_WIDTH = 350;
const TITLE_MARGIN_BOTTOM = 12;

export const NoPayloadDataWrapper = styled('div')({
	flex: 1,
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
});

export const NoPayloadDataContent = styled('div')(({ theme }) => ({
	display: 'grid',
	gridAutoFlow: 'row',
	justifyItems: 'center',
	rowGap: theme.spaces.xs,
	maxWidth: NO_PAYLOAD_DATA_MAX_WIDTH,

	color: theme.palette['graphics-primary-2'],

	svg: {
		width: IconSize.XL,
		height: IconSize.XL,
	},
}));

export const NoPayloadDataTitle = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
	textAlign: 'center',
	wordBreak: 'break-word',
	overflowWrap: 'break-word',
	whiteSpace: 'normal',

	marginBottom: TITLE_MARGIN_BOTTOM,
}));
