import { createContext, PropsWithChildren, useMemo, useState } from 'react';

import { assignAccessPaths } from '@/pages/Wiki/components/GroupSection/utils';
import { DraggableContextProps } from '@/pages/Wiki/context/types';
import {
	DropIndicatorPositionType,
	TreeNodes,
	Option,
} from '@/pages/Wiki/types';
import { extractIndexes } from '@/pages/Wiki/utils/extractIndexes';
import { extractItem } from '@/pages/Wiki/utils/extractItem';
import { getContainerPath } from '@/pages/Wiki/utils/getContainerPath';
import { isSiblingNode } from '@/pages/Wiki/utils/isSiblingNode';

export const DraggableContext = createContext({} as DraggableContextProps);

const { Top, Middle } = DropIndicatorPositionType;

export function DraggableProvider({ children }: PropsWithChildren) {
	const [rootNode, setAllNodes] = useState<TreeNodes>({ items: [] });

	const updateTree = (
		draggedNode: Option,
		hoveredNode: Option,
		position: DropIndicatorPositionType
	) => {
		const clonedNodes = structuredClone(rootNode);
		const areItemsInTheSameArray = isSiblingNode(draggedNode, hoveredNode);
		const { dragIndex, dropIndex } = extractIndexes(draggedNode, hoveredNode);

		const draggedItemParentPath = getContainerPath(draggedNode);
		const hoveredItemParentPath = getContainerPath(hoveredNode);

		const draggedItemParentContainer = draggedItemParentPath
			? extractItem(draggedItemParentPath, clonedNodes)
			: clonedNodes;
		const hoveredItemParentContainer = hoveredItemParentPath
			? extractItem(hoveredItemParentPath, clonedNodes)
			: clonedNodes;

		const draggedItem = extractItem(draggedNode.accessPath, clonedNodes);
		const hoveredItem = extractItem(hoveredNode.accessPath, clonedNodes);

		draggedItemParentContainer.splice(dragIndex, 1);

		let newPosition: number;

		if (position !== Middle) {
			if (areItemsInTheSameArray) {
				const index = hoveredItemParentContainer.findIndex(
					(item: Option) => item.id === hoveredItem.id
				);
				newPosition = position === Top ? index : index + 1;
			} else {
				newPosition = position === Top ? dropIndex : dropIndex + 1;
			}

			hoveredItemParentContainer.splice(newPosition, 0, draggedItem);
		} else if (hoveredItem.isGroup) {
			hoveredItem.items.push(draggedItem);
		}

		setAllNodes(() => ({
			items: assignAccessPaths(clonedNodes.items),
		}));
	};

	const contextValue = useMemo(
		() => ({
			updateTree,
			setCollapseData: setAllNodes,
			collapseData: rootNode,
		}),
		[rootNode, updateTree]
	);

	return (
		<DraggableContext.Provider value={contextValue}>
			{children}
		</DraggableContext.Provider>
	);
}
