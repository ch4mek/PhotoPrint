import styled from '@emotion/styled';

import { Tag } from '@/components/Tag';

const STATUS_WRAPPER_MAX_HEIGHT = 24;

export const RequestStatusWrapper = styled(Tag)(({ theme }) => ({
	maxHeight: STATUS_WRAPPER_MAX_HEIGHT,
	...theme.typography.body['s-400'],
}));
