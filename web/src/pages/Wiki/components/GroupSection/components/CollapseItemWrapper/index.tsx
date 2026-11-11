import { HTMLAttributes } from 'react';

import { CollapseItemContentWrapper } from '@/pages/Wiki/components/GroupSection/components/CollapseItemWrapper/styles';
import { CollapseItemWrapperProps } from '@/pages/Wiki/components/GroupSection/components/CollapseItemWrapper/types';
import { DraggableItem } from '@/pages/Wiki/components/GroupSection/components/DraggableItem';
import { COLLAPSE_ITEM_CLASSNAME } from '@/pages/Wiki/components/GroupSection/constants';

export function CollapseItemWrapper<T>({
	children,
	hasTopLevelStyles,
	item,
	isDragMode,
}: CollapseItemWrapperProps & HTMLAttributes<T>) {
	return (
		<DraggableItem item={item} isDragMode={isDragMode}>
			<CollapseItemContentWrapper
				className={COLLAPSE_ITEM_CLASSNAME}
				hasTopLevelStyles={hasTopLevelStyles && item?.parentGroupId == null}
			>
				{children}
			</CollapseItemContentWrapper>
		</DraggableItem>
	);
}
