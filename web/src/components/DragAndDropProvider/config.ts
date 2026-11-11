import { HTML5Backend } from 'react-dnd-html5-backend';
import { MouseTransition, TouchTransition } from 'react-dnd-multi-backend';
import { TouchBackend } from 'react-dnd-touch-backend';

import { DragAndDropBackendConfigType } from '@/components/DragAndDropProvider/types';

const mouseTransitionConfig: DragAndDropBackendConfigType = {
	id: 'html5',
	backend: HTML5Backend,
	transition: MouseTransition,
};

const touchTransitionConfig: DragAndDropBackendConfigType = {
	id: 'touch',
	backend: TouchBackend,
	options: {
		enableMouseEvents: true,
		delayTouchStart: 100,
		touchAngle: 45,
	},
	transition: TouchTransition,
};

export const dragAndDropBackendConfig = [
	mouseTransitionConfig,
	touchTransitionConfig,
] as const;
