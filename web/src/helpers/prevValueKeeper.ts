export class PrevValueKeeper<T> {
	value: T;

	constructor(initialValue: T) {
		this.value = initialValue;
	}

	set(newValue: T) {
		this.value = newValue;
	}

	get() {
		return this.value;
	}
}
