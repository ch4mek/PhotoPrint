import styled from '@emotion/styled';

import { TABLE_HEADER_HEIGHT } from '../../constants';
import { Row } from '../../styles';

export const TimelineCurrentDateWrapper = styled(Row)(({ theme }) => ({
	height: TABLE_HEADER_HEIGHT,
	position: 'sticky',
	paddingLeft: theme.spaces.s,
	left: 0,
	gap: theme.spaces.xs,
	borderBottom: `1px solid ${theme.palette['primary-4']}`,
}));
