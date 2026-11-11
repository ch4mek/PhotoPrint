import { createContext } from 'react';

const heightMap = new Map<number, { [key: number]: number }>();

export const HeightsContext = createContext(heightMap);
