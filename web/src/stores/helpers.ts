import { useShallow } from 'zustand/react/shallow';

export const createSelector = <T, U extends (keyof T)[]>(...args: U) =>
	useShallow((state: T): { [key in U[number]]: T[key] } =>
		args.reduce((res, curr) => {
			res[curr] = state[curr];

			return res;
		}, {} as { [key in U[number]]: T[key] })
	);
