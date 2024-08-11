import { StatusBar } from 'expo-status-bar';
import { StyleSheet, Text, View } from 'react-native';
import Welcome from './Components/Welcome';
import { Pet } from './Components/Pet';
import { PetQualities, Qualities } from './Components/PetQualities';

export default function App() {

  const petName = {
    firstName:"Roger",
    lastName:"Porticus"
  }


  const qualities:Qualities[] = [
    {
      qualOne:'A mammal?',
      qualTwo:'furry',
      qualThree:'lays eggs',
      age:14

    },
    {     
      qualOne:'A bird?',
      qualTwo:'furry',
      qualThree:'lays eggs',
      age:14
    }
  ]

  const message=""

  return (
    <View style={styles.container}>
      <Welcome name="Martin" age={54} gender={true}/>
      <Pet petName={petName} type='Platypus' />
      <PetQualities qualities={qualities} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    alignItems: 'flex-end',
    justifyContent: 'flex-end',
    padding:30
  },
});
