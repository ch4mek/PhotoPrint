import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { CLIENT_PROJECTS_CELL_FONT_SIZE } from '@/components/Grid/constants';

const CLIENT_PROJECTS_CELL_HEIGHT = 17;

export const Content = styled(Typography)({
	display: 'inline-block',
	height: '100%',
	verticalAlign: 'text-top',
	overflow: 'hidden',
	width: '100%',
	lineHeight: `${CLIENT_PROJECTS_CELL_HEIGHT}px`,
	fontSize: `${CLIENT_PROJECTS_CELL_FONT_SIZE}px`,
});
