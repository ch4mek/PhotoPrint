import { StateCreator } from 'zustand';
import { devtools } from 'zustand/middleware';
import { immer } from 'zustand/middleware/immer';

type MiddlewareMutators = [
	['zustand/devtools', never],
	['zustand/immer', never]
];

export const defaultMiddleware = <T>(
	initializer: StateCreator<T, MiddlewareMutators>
) => devtools(immer(initializer));
