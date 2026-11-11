import styled from '@emotion/styled';

const CHILDREN_WRAPPER_TRANSITION = 'height 233ms cubic-bezier(0.4, 0, 0.2, 1)';

export const ChildrenWrapper = styled('div')({
	height: 0,
	overflow: 'hidden',
	transition: CHILDREN_WRAPPER_TRANSITION,
});
