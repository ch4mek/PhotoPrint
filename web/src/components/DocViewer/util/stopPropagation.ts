import { MouseEvent } from 'react';

export const stopPropagation = <T extends HTMLElement>(e: MouseEvent<T>) =>
	e.stopPropagation();
