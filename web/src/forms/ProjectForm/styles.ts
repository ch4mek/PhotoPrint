import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { HALF_OPACITY } from '@/constants/opacities';

const POPUP_TITLE_WIDTH = 128;

export const ProjectInfo = styled(Typography)({
	maxWidth: `calc(100% - ${POPUP_TITLE_WIDTH}px)`,
	width: '100%',
	display: 'flex',
	flexWrap: 'nowrap',
	justifyContent: 'flex-start',
});

export const ProjectName = styled('span')(({ theme }) => ({
	maxWidth: '50%',
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	textWrap: 'nowrap',
	marginRight: theme.spaces.xs,
}));

export const ClientName = styled(ProjectName)({
	opacity: HALF_OPACITY,
	marginRight: 0,
});

export const TabWrapper = styled('div')<{ height: string }>(({ height }) => ({
	height,
}));
