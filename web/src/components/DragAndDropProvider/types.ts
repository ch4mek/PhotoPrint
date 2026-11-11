import type { BackendFactory } from 'dnd-core/dist/interfaces';
import { Transition } from 'react-dnd-multi-backend';

export type DragAndDropBackendConfigType = {
	id: string;
	backend: BackendFactory;
	transition: Transition;
	options?: Record<string, unknown>;
};
