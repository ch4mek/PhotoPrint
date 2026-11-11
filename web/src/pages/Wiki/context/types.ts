import { Dispatch, SetStateAction } from 'react';

import {
	DropIndicatorPositionType,
	TreeNodes,
	Option,
} from '@/pages/Wiki/types';

export interface DraggableContextProps {
	updateTree: (
		draggedNode: Option,
		hoveredNode: Option,
		position: DropIndicatorPositionType
	) => void;
	setCollapseData: Dispatch<SetStateAction<TreeNodes>>;
	collapseData: TreeNodes;
}

export interface WikiContextValues {
	isEditorReadOnly: boolean;
	setToggleEditorReadOnly: VoidFunction;
	setIsEditorReadOnly: Dispatch<SetStateAction<boolean>>;
}
