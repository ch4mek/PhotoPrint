import styled from '@emotion/styled';

import { TabWrapperProps } from '../../types';

export const TabWrapper = styled('div')<TabWrapperProps>(({ height }) => ({
	height,
}));
