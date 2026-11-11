import { PropsWithChildren } from 'react';
import { DndProvider } from 'react-dnd';
import { MultiBackend } from 'react-dnd-multi-backend';

import { dragAndDropBackendConfig } from '@/components/DragAndDropProvider/config';

export function DragAndDropProvider({ children }: PropsWithChildren) {
	return (
		<DndProvider
			backend={MultiBackend}
			options={{
				backends: dragAndDropBackendConfig,
			}}
		>
			{children}
		</DndProvider>
	);
}
