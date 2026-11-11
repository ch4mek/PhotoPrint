import type { Resources } from 'i18next';

export type AllowedKeys<T> = Record<string, T>;

type HomeResource = Resources['home'];
export type GreetingsKey = keyof HomeResource['greetings'];
export type DevelopmentPlanStatusKeys =
	keyof Resources['common']['developmentPlanStatus'];
